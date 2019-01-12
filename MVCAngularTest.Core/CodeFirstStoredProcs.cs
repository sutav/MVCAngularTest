using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;

namespace MVCAngularTest.Core
{
    /// <summary>
    /// Holds multiple Result Sets returned from a Stored Procedure call. 
    /// </summary>
    public class ResultsList : IEnumerable
    {
        // our internal object that is the list of results lists
        List<List<object>> thelist = new List<List<object>>();

        /// <summary>
        /// Add a results list to the results set
        /// </summary>
        /// <param name="list"></param>
        public void Add(List<object> list)
        {
            thelist.Add(list);
        }

        /// <summary>
        /// Return an enumerator over the internal list
        /// </summary>
        /// <returns>Enumerator over List<object> that make up the result sets </returns>
        public IEnumerator GetEnumerator()
        {
            return thelist.GetEnumerator();
        }

        /// <summary>
        /// Return the count of result sets
        /// </summary>
        public Int32 Count
        {
            get { return thelist.Count; }
        }

        /// <summary>
        /// Get the nth results list item
        /// </summary>
        /// <param name="index"></param>
        /// <returns>List of objects that make up the result set</returns>
        public List<object> this[int index]
        {
            get { return thelist[index]; }
        }

        /// <summary>
        /// Return the result set that contains a particular type and does a cast to that type.
        /// </summary>
        /// <typeparam name="T">Type that was listed in StoredProc object as a possible return type for the stored procedure</typeparam>
        /// <returns>List of T; if no results match, returns an empty list</returns>
        public List<T> ToList<T>()
        {
            // search each non-empty results list 
            foreach (List<object> list in thelist.Where(p => p.Count > 0).Select(p => p))
            {
                // compare types of the first element - this is why we filter for non-empty results
                if (typeof(T) == list[0].GetType())
                {
                    // do cast to return type
                    return list.Cast<T>().Select(p => p).ToList();
                }
            }

            // no matches? return empty list
            return new List<T>();
        }

        /// <summary>
        /// Return the result set that contains a particular type and does a cast to that type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Array of T; if no results match, returns an empty array</returns>
        public T[] ToArray<T>()
        {
            // search each non-empty results list 
            foreach (List<object> list in thelist.Where(p => p.Count > 0).Select(p => p))
            {
                // compare types of the first element - this is why we filter for non-empty results
                if (typeof(T) == list[0].GetType())
                {
                    // do cast to return type
                    return list.Cast<T>().Select(p => p).ToArray();
                }
            }

            // no matches? return empty array
            return new T[0];
        }
    }

    /// <summary>
    /// Genericized version of StoredProc object, takes a .Net POCO object type for the parameters. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StoredProc<T> : StoredProc
    {
        /// <summary>
        /// Constructor. Note that the return type objects must have a default constructor!
        /// </summary>
        /// <param name="types">Types returned by the stored procedure. Order is important!</param>
        public StoredProc(params Type[] types)
        {
            // set default schema
            schema = "dbo";

            // allow override by attribute
            var schema_attr = typeof(T).GetAttribute<StoredProcAttributes.Schema>();
            if (null != schema_attr)
                schema = schema_attr.Value;

            // set default proc name
            procname = typeof(T).Name;

            // allow override by attribute
            var procname_attr = typeof(T).GetAttribute<StoredProcAttributes.Name>();
            if (null != procname_attr)
                procname = procname_attr.Value;

            outputtypes.AddRange(types);
        }

        /// <summary>
        /// Contains a mapping of property names to parameter names. We do this since this mapping is complex; 
        /// i.e. the default parameter name may be overridden by the Name attribute
        /// </summary>
        internal Dictionary<String, String> MappedParams = new Dictionary<string, string>();

        /// <summary>
        /// Store output parameter values back into the data object
        /// </summary>
        /// <param name="parms">List of parameters</param>
        /// <param name="data">Source data object</param>
        internal void ProcessOutputParms(IEnumerable<SqlParameter> parms, T data)
        {
            // get the list of properties for this type
            PropertyInfo[] props = typeof(T).GetMappedProperties();

            // we want to write data back to properties for every non-input only parameter
            foreach (SqlParameter parm in parms
                .Where(p => p.Direction != ParameterDirection.Input)
                .Select(p => p))
            {
                // get the property name mapped to this parameter
                String propname = MappedParams.Where(p => p.Key == parm.ParameterName).Select(p => p.Value).First();

                // extract the matchingproperty and set its value
                PropertyInfo prop = props.Where(p => p.Name == propname).FirstOrDefault();
                prop.SetValue(data, parm.Value, null);
            }
        }

        /// <summary>
        /// Convert parameters from type T properties to SqlParameters
        /// </summary>
        /// <param name="data">Source data object</param>
        /// <returns></returns>
        internal IEnumerable<SqlParameter> Parameters(T data)
        {
            // clear the parameter to property mapping since we'll be recreating this
            MappedParams.Clear();

            // list of parameters we'll be returning
            List<SqlParameter> parms = new List<SqlParameter>();

            // properties that we're converting to parameters are everything without
            // a NotMapped attribute
            foreach (PropertyInfo p in typeof(T).GetMappedProperties())
            {
                //---------------------------------------------------------------------------------
                // process attributes
                //---------------------------------------------------------------------------------

                // create parameter and store default name - property name
                SqlParameter holder = new SqlParameter()
                {
                    ParameterName = p.Name
                };

                // override of parameter name by attribute
                var name = p.GetAttribute<StoredProcAttributes.Name>();
                if (null != name)
                    holder.ParameterName = name.Value;

                // save direction (default is input)
                var dir = p.GetAttribute<StoredProcAttributes.Direction>();
                if (null != dir)
                    holder.Direction = dir.Value;

                // save size
                var size = p.GetAttribute<StoredProcAttributes.Size>();
                if (null != size)
                    holder.Size = size.Value;

                // save database type of parameter
                var parmtype = p.GetAttribute<StoredProcAttributes.ParameterType>();
                if (null != parmtype)
                    holder.SqlDbType = parmtype.Value;

                // save user-defined type name
                var typename = p.GetAttribute<StoredProcAttributes.TypeName>();
                if (null != typename)
                    holder.TypeName = typename.Value;

                // save precision
                var precision = p.GetAttribute<StoredProcAttributes.Precision>();
                if (null != precision)
                    holder.Precision = precision.Value;

                // save scale
                var scale = p.GetAttribute<StoredProcAttributes.Scale>();
                if (null != scale)
                    holder.Scale = scale.Value;

                //---------------------------------------------------------------------------------
                // Save parameter value
                //---------------------------------------------------------------------------------

                // store table values, scalar value or null
                var value = p.GetValue(data, null);
                if (value == null)
                {
                    // set database null marker for null value
                    holder.Value = DBNull.Value;
                }
                else if (SqlDbType.Structured == holder.SqlDbType)
                {
                    // catcher - tvp must be ienumerable type
                    if (!(value is IEnumerable))
                        throw new InvalidCastException(String.Format("{0} must be an IEnumerable Type", p.Name));

                    // ge the type underlying the IEnumerable
                    Type basetype = CodeFirstStoredProcHelpers.GetUnderlyingType(value.GetType());

                    // get the table valued parameter table type name
                    var schema = p.GetAttribute<StoredProcAttributes.Schema>();
                    if (null == schema && null != basetype)
                        schema = basetype.GetAttribute<StoredProcAttributes.Schema>();

                    var tvpname = p.GetAttribute<StoredProcAttributes.TableName>();
                    if (null == tvpname && null != basetype)
                        tvpname = basetype.GetAttribute<StoredProcAttributes.TableName>();

                    holder.TypeName = (null != schema) ? schema.Value : "dbo";
                    holder.TypeName += ".";
                    holder.TypeName += (null != tvpname) ? tvpname.Value : p.Name;

                    // generate table valued parameter
                    holder.Value = CodeFirstStoredProcHelpers.TableValuedParameter((IList)value);
                }
                else
                {
                    // process normal scalar value
                    holder.Value = value;
                }

                // save the mapping between the parameter name and property name, since the parameter
                // name can be overridden
                MappedParams.Add(holder.ParameterName, p.Name);

                // add parameter to list
                parms.Add(holder);
            }

            return parms;
        }

        /// <summary>
        /// Fluent API - assign owner (schema)
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public new StoredProc<T> HasOwner(String owner)
        {
            base.HasOwner(owner);
            return this;
        }

        /// <summary>
        /// Fluent API - assign procedure name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new StoredProc<T> HasName(String name)
        {
            base.HasName(name);
            return this;
        }

        /// <summary>
        /// Fluent API - set the data types of resultsets returned by the stored procedure. 
        /// Order is important! Note that the return type objects must have a default constructor!
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public new StoredProc<T> ReturnsTypes(params Type[] types)
        {
            base.ReturnsTypes(types);
            return this;
        }
    }

    /// <summary>
    /// Represents a Stored Procedure in the database. Note that the return type objects
    /// must have a default constructor!
    /// </summary>
    public class StoredProc
    {
        /// <summary>
        /// Database owner of this object
        /// </summary>
        public String schema { get; set; }

        /// <summary>
        /// Name of the stored procedure
        /// </summary>
        public String procname { get; set; }

        /// <summary>
        /// Fluent API - assign owner (schema)
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public StoredProc HasOwner(String owner)
        {
            schema = owner;
            return this;
        }

        /// <summary>
        /// Fluent API - assign procedure name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StoredProc HasName(String name)
        {
            procname = name;
            return this;
        }

        /// <summary>
        /// Fluent API - set the data types of resultsets returned by the stored procedure. 
        /// Order is important! Note that the return type objects must have a default constructor!
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public StoredProc ReturnsTypes(params Type[] types)
        {
            outputtypes.AddRange(types);
            return this;
        }

        /// <summary>
        /// Get the fully (schema plus owner) name of the stored procedure
        /// </summary>
        internal String fullname
        {
            get { return schema + "." + procname; }
        }

        /// <summary>
        /// Constructors. Note that the return type objects
        /// must have a default constructor!
        /// </summary>
        public StoredProc()
        {
            schema = "dbo";
        }

        public StoredProc(String name)
        {
            schema = "dbo";
            procname = name;
        }

        public StoredProc(String name, params Type[] types)
        {
            schema = "dbo";
            procname = name;
            outputtypes.AddRange(types);
        }

        public StoredProc(String owner, String name, params Type[] types)
        {
            schema = owner;
            procname = name;
            outputtypes.AddRange(types);
        }

        /// <summary>
        /// List of data types that this stored procedure returns as result sets. 
        /// Order is important!
        /// </summary>
        internal List<Type> outputtypes = new List<Type>();

        /// <summary>
        /// Get an array of types returned
        /// </summary>
        internal Type[] returntypes
        {
            get { return outputtypes.ToArray(); }
        }
    }

    /// <summary>
    /// Contains extension methods to Code First database objects for Stored Procedure processing
    /// </summary>
    public static class CodeFirstStoredProcs
    {
        /// <summary>
        /// Generic Typed version of calling a stored procedure
        /// </summary>
        /// <typeparam name="T">Type of object containing the parameter data</typeparam>
        /// <param name="context">Database Context to use for the call</param>
        /// <param name="procedure">Generic Typed stored procedure object</param>
        /// <param name="data">The actual object containing the parameter data</param>
        /// <returns></returns>
        public static void ExecuteStoredProcNonQuery<T>(this DbContext context, StoredProc<T> procedure, T data)
        {
            IEnumerable<SqlParameter> parms = procedure.Parameters(data);
            ResultsList results = context.ReadFromStoredProc(procedure.fullname, parms, procedure.returntypes);
            procedure.ProcessOutputParms(parms, data);

        }
        /// <summary>
        /// Generic Typed version of calling a stored procedure
        /// </summary>
        /// <typeparam name="T">Type of object containing the parameter data</typeparam>
        /// <param name="context">Database Context to use for the call</param>
        /// <param name="procedure">Generic Typed stored procedure object</param>
        /// <param name="data">The actual object containing the parameter data</param>
        /// <returns></returns>
        public static int ExecuteStoredProcNonQueryWithReturnValue<T>(this DbContext context, StoredProc<T> procedure, T data)
        {
            IEnumerable<SqlParameter> parms = procedure.Parameters(data);
            ResultsList results = context.ReadFromStoredProc(procedure.fullname, parms, procedure.returntypes);
            procedure.ProcessOutputParms(parms, data);
            return (int)parms.Where(p => p.Direction == ParameterDirection.ReturnValue).SingleOrDefault().Value;
        }

        /// <summary>
        /// Generic Typed version of calling a stored procedure
        /// </summary>
        /// <typeparam name="T">Type of object containing the parameter data</typeparam>
        /// <param name="context">Database Context to use for the call</param>
        /// <param name="procedure">Generic Typed stored procedure object</param>
        /// <param name="data">The actual object containing the parameter data</param>
        /// <returns></returns>
        public static ResultsList CallStoredProc<T>(this DbContext context, StoredProc<T> procedure, T data)
        {
            IEnumerable<SqlParameter> parms = procedure.Parameters(data);
            ResultsList results = context.ReadFromStoredProc(procedure.fullname, parms, procedure.returntypes);
            procedure.ProcessOutputParms(parms, data);
            return results ?? new ResultsList();
        }

        /// <summary>
        /// Call a stored procedure, passing in the stored procedure object and a list of parameters
        /// </summary>
        /// <param name="context">Database context used for the call</param>
        /// <param name="procedure">Stored Procedure</param>
        /// <param name="parms">List of parameters</param>
        /// <returns></returns>
        public static ResultsList CallStoredProc(this DbContext context, StoredProc procedure, IEnumerable<SqlParameter> parms = null)
        {
            ResultsList results = context.ReadFromStoredProc(procedure.fullname, parms, procedure.returntypes);
            return results ?? new ResultsList();
        }

        /// <summary>
        /// internal
        /// 
        /// Call a stored procedure and get results back. 
        /// </summary>
        /// <param name="context">Code First database context object</param>
        /// <param name="tablename">Qualified name of proc to call</param>
        /// <param name="parms">List of ParameterHolder objects - input and output parameters</param>
        /// <param name="outputtypes">List of types to expect in return. Each type *must* have a default constructor.</param>
        /// <returns></returns>
        internal static ResultsList ReadFromStoredProc(this DbContext context,
            String procname,
            IEnumerable<SqlParameter> parms = null,
            params Type[] outputtypes)
        {
            // create our output set object
            ResultsList results = new ResultsList();

            // ensure that we have a type list, even if it's empty
            IEnumerator currenttype = (null == outputtypes) ?
                new Type[0].GetEnumerator() :
                outputtypes.GetEnumerator();

            // handle to the database connection object
            var connection = (SqlConnection)context.Database.Connection;
            try
            {
                // open the connect for use and create a command object
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    // command to execute is our stored procedure
                    cmd.CommandText = procname;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // move parameters to command object
                    if (null != parms)
                        foreach (SqlParameter p in parms)
                            cmd.Parameters.Add(p);
                    //    foreach (ParameterHolder p in parms)
                    //        cmd.Parameters.Add(p.toParameter(cmd));

                    // Do It! This actually makes the database call
                    var reader = cmd.ExecuteReader();

                    // get the type we're expecting for the first result. If no types specified,
                    // ignore all results
                    if (currenttype.MoveNext())
                    {
                        // process results - repeat this loop for each result set returned by the stored proc
                        // for which we have a result type specified
                        do
                        {
                            // get properties to save for the current destination type
                            PropertyInfo[] props = ((Type)currenttype.Current).GetMappedProperties();

                            // create a destination for our results
                            List<object> current = new List<object>();

                            // process the result set
                            while (reader.Read())
                            {
                                // create an object to hold this result
                                object item = ((Type)currenttype.Current).GetConstructor(System.Type.EmptyTypes).Invoke(new object[0]);

                                // copy data elements by parameter name from result to destination object
                                reader.ReadRecord(item, props);

                                // add newly populated item to our output list
                                current.Add(item);
                            }

                            // add this result set to our return list
                            results.Add(current);
                        }
                        while (reader.NextResult() && currenttype.MoveNext());
                    }
                    // close up the reader, we're done saving results
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading from stored proc " + procname + ": " + ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }

            return results;
        }
    }

    /// <summary>
    /// Contains extension methods to Code First database objects for Stored Procedure processing
    /// </summary>
    internal static class CodeFirstStoredProcHelpers
    {
        /// <summary>
        /// Get the underlying class type for lists, etc. that implement IEnumerable<>.
        /// </summary>
        /// <param name="listtype"></param>
        /// <returns></returns>
        public static Type GetUnderlyingType(Type listtype)
        {
            Type basetype = null;
            foreach (Type i in listtype.GetInterfaces())
                if (i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                    basetype = i.GetGenericArguments()[0];

            return basetype;
        }
        /// <summary>
        /// Get properties of a type that do not have the 'NotMapped' attribute
        /// </summary>
        /// <param name="t">Type to examine for properites</param>
        /// <returns>Array of properties that can be filled</returns>
        public static PropertyInfo[] GetMappedProperties(this Type t)
        {
            var props1 = t.GetProperties();
            var props2 = props1
                .Where(p => p.GetAttribute<NotMappedAttribute>() == null)
                .Select(p => p);
            return props2.ToArray();
        }

        /// <summary>
        /// Get an attribute for a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo"></param>
        /// <param name="customAttribute"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Type type)
            where T : Attribute
        {
            var attributes = type.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            return (T)attributes;
        }

        /// <summary>
        /// Get an attribute for a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo"></param>
        /// <param name="customAttribute"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this PropertyInfo propertyinfo)
            where T : Attribute
        {
            var attributes = propertyinfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            return (T)attributes;
        }

        /// <summary>
        /// Read data for the current result row from a reader into a destination object, by the name
        /// of the properties on the destination object.
        /// </summary>
        /// <param name="reader">data reader holding return data</param>
        /// <param name="t">object to populate</param>
        /// <returns></returns>
        public static object ReadRecord(this DbDataReader reader, object t, PropertyInfo[] props)
        {
            String name;

            // copy mapped properties
            foreach (PropertyInfo p in props)
            {
                try
                {
                    // default name is property name, override of parameter name by attribute
                    var attr = p.GetAttribute<StoredProcAttributes.Name>();
                    name = (null == attr) ? p.Name : attr.Value;

                    // get the requested value from the returned dataset and handle null values
                    var data = reader[name];
                    if (data.GetType() == typeof(System.DBNull))
                        p.SetValue(t, null, null);
                    else
                        p.SetValue(t, reader[name], null);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IndexOutOfRangeException))
                    {
                        // if the result set doesn't have this value, intercept the exception
                        // and set the property value to null / 0
                        p.SetValue(t, null, null);
                    }
                    else
                        // something bad happened, pass on the exception
                        throw ex;
                }
            }

            return t;
        }

        /// <summary>
        /// Read data for the current result row from a reader into a destination object, by the name
        /// of the properties on the destination object.
        /// </summary>
        /// <param name="reader">data reader holding return data</param>
        /// <param name="t">object to populate</param>
        /// <returns></returns>
        public static object ReadRecord(this SqlDataReader reader, object t, PropertyInfo[] props)
        {
            String name = "";

            // copy mapped properties
            foreach (PropertyInfo p in props)
            {
                try
                {
                    // default name is property name, override of parameter name by attribute
                    var attr = p.GetAttribute<StoredProcAttributes.Name>();
                    name = (null == attr) ? p.Name : attr.Value;

                    // get the requested value from the returned dataset and handle null values
                    var data = reader[name];
                    if (data.GetType() == typeof(System.DBNull))
                        p.SetValue(t, null, null);
                    else
                        p.SetValue(t, reader[name], null);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IndexOutOfRangeException))
                    {
                        // if the result set doesn't have this value, intercept the exception
                        // and set the property value to null / 0
                        p.SetValue(t, null, null);
                    }
                    else
                    {
                        // tell the user *where* we had an exception
                        Exception outer = new Exception(String.Format("Exception processing return column {0} in {1}",
                            name, t.GetType().Name), ex);

                        // something bad happened, pass on the exception
                        throw outer;
                    }
                }
            }

            return t;
        }

        /// <summary>
        /// Do the work of converting a source data object to SqlDataRecords 
        /// using the parameter attributes to create the table valued parameter definition
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<SqlDataRecord> TableValuedParameter(IList table)
        {
            // get the object type underlying our table
            Type t = CodeFirstStoredProcHelpers.GetUnderlyingType(table.GetType());

            // list of converted values to be returned to the caller
            List<SqlDataRecord> recordlist = new List<SqlDataRecord>();

            // get all mapped properties
            PropertyInfo[] props = CodeFirstStoredProcHelpers.GetMappedProperties(t);

            // get the column definitions, into an array
            List<SqlMetaData> columnlist = new List<SqlMetaData>();

            // get the propery column name to property name mapping
            // and generate the SqlMetaData for each property/column
            Dictionary<String, String> mapping = new Dictionary<string, string>();
            foreach (PropertyInfo p in props)
            {
                // default name is property name, override of parameter name by attribute
                var attr = p.GetAttribute<StoredProcAttributes.Name>();
                String name = (null == attr) ? p.Name : attr.Value;
                mapping.Add(name, p.Name);

                // get column type
                var ct = p.GetAttribute<StoredProcAttributes.ParameterType>();
                SqlDbType coltype = (null == ct) ? SqlDbType.Int : ct.Value;

                // create metadata column definition
                SqlMetaData column;
                switch (coltype)
                {
                    case SqlDbType.Binary:
                    case SqlDbType.Char:
                    case SqlDbType.NChar:
                    case SqlDbType.Image:
                    case SqlDbType.VarChar:
                    case SqlDbType.NVarChar:
                    case SqlDbType.Text:
                    case SqlDbType.NText:
                    case SqlDbType.VarBinary:
                        // get column size
                        var sa = p.GetAttribute<StoredProcAttributes.Size>();
                        int size = (null == sa) ? 50 : sa.Value;
                        column = new SqlMetaData(name, coltype, size);
                        break;

                    case SqlDbType.Decimal:
                        // get column precision and scale
                        var pa = p.GetAttribute<StoredProcAttributes.Precision>();
                        Byte precision = (null == pa) ? (byte)10 : pa.Value;
                        var sca = p.GetAttribute<StoredProcAttributes.Scale>();
                        Byte scale = (null == sca) ? (byte)2 : sca.Value;
                        column = new SqlMetaData(name, coltype, precision, scale);
                        break;

                    default:
                        column = new SqlMetaData(name, coltype);
                        break;
                }

                // Add metadata to column list
                columnlist.Add(column);
            }

            // load each object in the input data table into sql data records
            foreach (object s in table)
            {
                // create the sql data record using the column definition
                SqlDataRecord record = new SqlDataRecord(columnlist.ToArray());
                for (int i = 0; i < columnlist.Count(); i++)
                {
                    // locate the value of the matching property
                    var value = props.Where(p => p.Name == mapping[columnlist[i].Name])
                        .First()
                        .GetValue(s, null);

                    // set the value
                    record.SetValue(i, value);
                }

                // add the sql data record to our output list
                recordlist.Add(record);
            }

            // return our list of data records
            return recordlist;
        }
    }

    /// <summary>
    /// Contains attributes for Stored Procedure processing
    /// </summary>
    public class StoredProcAttributes
    {
        /// <summary>
        /// Parameter name override. Default value for parameter name is the name of the 
        /// property. This overrides that default with a user defined name.
        /// </summary>
        public class Name : Attribute
        {
            public String Value { get; set; }

            public Name(String s)
                : base()
            {
                Value = s;
            }
        }

        /// <summary>
        /// Size in bytes of returned data. Should be used on output and returncode parameters.
        /// </summary>
        public class Size : Attribute
        {
            public Int32 Value { get; set; }

            public Size(Int32 s)
                : base()
            {
                Value = s;
            }
        }

        /// <summary>
        /// Size in bytes of returned data. Should be used on output and returncode parameters.
        /// </summary>
        public class Precision : Attribute
        {
            public Byte Value { get; set; }

            public Precision(Byte s)
                : base()
            {
                Value = s;
            }
        }

        /// <summary>
        /// Size in bytes of returned data. Should be used on output and returncode parameters.
        /// </summary>
        public class Scale : Attribute
        {
            public Byte Value { get; set; }

            public Scale(Byte s)
                : base()
            {
                Value = s;
            }
        }

        /// <summary>
        /// Defines the direction of data flow for the property/parameter.
        /// </summary>
        public class Direction : Attribute
        {
            public ParameterDirection Value { get; set; }

            public Direction(ParameterDirection d)
            {
                Value = d;
            }
        }

        /// <summary>
        /// Define the SqlDbType for the parameter corresponding to this property.
        /// </summary>
        public class ParameterType : Attribute
        {
            public SqlDbType Value { get; set; }

            public ParameterType(SqlDbType t)
            {
                Value = t;
            }
        }

        /// <summary>
        /// Allows the setting of the parameter type name for user defined types in the database
        /// </summary>
        public class TypeName : Attribute
        {
            public String Value { get; set; }

            public TypeName(String t)
            {
                Value = t;
            }
        }

        /// <summary>
        /// Allows the setting of the user defined table type name for table valued parameters
        /// </summary>
        public class TableName : Attribute
        {
            public String Value { get; set; }

            public TableName(String t)
            {
                Value = t;
            }
        }

        /// <summary>
        /// Allows the setting of the user defined table type name for table valued parameters
        /// </summary>
        public class Schema : Attribute
        {
            public String Value { get; set; }

            public Schema(String t)
            {
                Value = t;
            }
        }
    }
}
