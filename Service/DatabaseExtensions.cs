using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Service
{
    /// <summary>
    /// 查詢動態類
    /// add yuangang by 2016-05-10
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// 自訂Connection對象
        /// </summary>
        private static IDbConnection DefaultConnection
        {
            get
            {
                return Domain.MyConfig.DefaultConnection;
            }
        }
        /// <summary>
        /// 自訂資料庫連接字串，與EF連接模式一致
        /// </summary>
        private static string DefaultConnectionString
        {
            get
            {
                return Domain.MyConfig.DefaultConnectionString;
            }
        }
        /// <summary>
        /// 動態查詢主方法
        /// </summary>
        /// <returns></returns>
        public static IEnumerable SqlQueryForDynamic(this Database db,
                string sql,
                params object[] parameters)
        {
            IDbConnection defaultConn = DefaultConnection;

            //ADO.NET資料庫連接字串
            db.Connection.ConnectionString = DefaultConnectionString;

            return SqlQueryForDynamicOtherDB(db, sql, defaultConn, parameters);
        }
        private static IEnumerable SqlQueryForDynamicOtherDB(this Database db, string sql, IDbConnection conn, params object[] parameters)
        {
            conn.ConnectionString = db.Connection.ConnectionString;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
            }

            using (IDataReader dataReader = cmd.ExecuteReader())
            {

                if (!dataReader.Read())
                {
                    return null; //無結果返回Null
                }

                #region 構建動態欄位

                TypeBuilder builder = DatabaseExtensions.CreateTypeBuilder(
                    "EF_DynamicModelAssembly",
                    "DynamicModule",
                    "DynamicType");

                int fieldCount = dataReader.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    Type t = dataReader.GetFieldType(i);
                    switch (t.Name.ToLower())
                    {
                        case "decimal":
                            t = typeof(Decimal?);
                            break;
                        case "double":
                            t = typeof(Double?);
                            break;
                        case "datetime":
                            t = typeof(DateTime?);
                            break;
                        case "single":
                            t = typeof(float?);
                            break;
                        case "int16":
                            t = typeof(int?);
                            break;
                        case "int32":
                            t = typeof(int?);
                            break;
                        case "int64":
                            t = typeof(int?);
                            break;
                        default:
                            break;
                    }
                    DatabaseExtensions.CreateAutoImplementedProperty(
                        builder,
                        dataReader.GetName(i),
                        t);
                }

                #endregion

                cmd.Parameters.Clear();
                dataReader.Close();
                dataReader.Dispose();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();

                Type returnType = builder.CreateType();

                if (parameters != null)
                {
                    return db.SqlQuery(returnType, sql, parameters);
                }
                else
                {
                    return db.SqlQuery(returnType, sql);
                }
            }
        }

        private static TypeBuilder CreateTypeBuilder(string assemblyName, string moduleName, string typeName)
        {
            TypeBuilder typeBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
              new AssemblyName(assemblyName),
              AssemblyBuilderAccess.Run).DefineDynamicModule(moduleName).DefineType(typeName,
              TypeAttributes.Public);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
            return typeBuilder;
        }

        private static void CreateAutoImplementedProperty(TypeBuilder builder, string propertyName, Type propertyType)
        {
            const string PrivateFieldPrefix = "m_";
            const string GetterPrefix = "get_";
            const string SetterPrefix = "set_";

            // Generate the field.
            FieldBuilder fieldBuilder = builder.DefineField(
              string.Concat(
                PrivateFieldPrefix, propertyName),
              propertyType,
              FieldAttributes.Private);

            // Generate the property
            PropertyBuilder propertyBuilder = builder.DefineProperty(
              propertyName,
              System.Reflection.PropertyAttributes.HasDefault,
              propertyType, null);

            // Property getter and setter attributes.
            MethodAttributes propertyMethodAttributes = MethodAttributes.Public
              | MethodAttributes.SpecialName
              | MethodAttributes.HideBySig;

            // Define the getter method.
            MethodBuilder getterMethod = builder.DefineMethod(
                string.Concat(
                  GetterPrefix, propertyName),
                propertyMethodAttributes,
                propertyType,
                Type.EmptyTypes);

            // Emit the IL code.
            // ldarg.0
            // ldfld,_field
            // ret
            ILGenerator getterILCode = getterMethod.GetILGenerator();
            getterILCode.Emit(OpCodes.Ldarg_0);
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILCode.Emit(OpCodes.Ret);

            // Define the setter method.
            MethodBuilder setterMethod = builder.DefineMethod(
              string.Concat(SetterPrefix, propertyName),
              propertyMethodAttributes,
              null,
              new Type[] { propertyType });

            // Emit the IL code.
            // ldarg.0
            // ldarg.1
            // stfld,_field
            // ret
            ILGenerator setterILCode = setterMethod.GetILGenerator();
            setterILCode.Emit(OpCodes.Ldarg_0);
            setterILCode.Emit(OpCodes.Ldarg_1);
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
            setterILCode.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getterMethod);
            propertyBuilder.SetSetMethod(setterMethod);
        }

        public static dynamic SqlFunctionForDynamic(this Database db,
                string sql,
                params object[] parameters)
        {
            IDbConnection conn = DefaultConnection;

            //ADO.NET資料庫連接字串
            conn.ConnectionString = DefaultConnectionString;

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
            }
            //1、DataReader查詢資料
            using (IDataReader dataReader = cmd.ExecuteReader())
            {
                if (!dataReader.Read())
                {
                    return null;
                }
                //2、DataReader轉換Json
                string jsonstr = Common.JsonHelper.JsonConverter.ToJson(dataReader);
                dataReader.Close();
                dataReader.Dispose();
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
                //3、Json轉換動態類
                dynamic dyna = Common.JsonHelper.JsonConverter.ConvertJson(jsonstr);
                return dyna;
            }
        }
        /// <summary>
        /// 對可空類型進行判斷轉換(*要不然會報錯)
        /// </summary>
        /// <param name="value">DataReader欄位的值</param>
        /// <param name="conversionType">該欄位的類型</param>
        /// <returns></returns>
        private static object CheckType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;
                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 判斷指定物件是否是有效值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsNullOrDBNull(object obj)
        {
            return (obj == null || (obj is DBNull)) ? true : false;
        }
    }
}