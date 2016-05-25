using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BondConverter
{
    public class BondConverter
    {
        private Dictionary<Type, string> bondMapping = new Dictionary<Type, string>();

        public void AddTypes(IEnumerable<Type> types)
        {
            foreach (var t in types)//.Where(x => Attribute.IsDefined(x, typeof(DataContractAttribute))))
            {
                GenerateBondStructs(t);
            }
        }

        public string GenerateBondFile(string targetNamespace)
        {
            StringBuilder builder = new StringBuilder();

            //Write a namespace
            builder.AppendLine(string.Format("namespace {0}", targetNamespace));
            builder.AppendLine();
            foreach (var b in bondMapping.Values)
            {
                builder.Append(b);
                builder.AppendLine();
            }

            return builder.ToString();
        }

        public BondConverter()
        {

        }

        public BondConverter(IEnumerable<Type> types)
        {
            AddTypes(types);
        }

        private void GenerateBondStructs(Type t)
        {
            if (bondMapping.ContainsKey(t))
                return; //Already mapped

            StringBuilder str = new StringBuilder();

            str.AppendLine(GenerateStructDeclaration(t));
            str.AppendLine("{");

            //Append members
            GenerateStructMembers(t).ToList().ForEach(x => str.AppendLine(x));

            str.AppendLine("};");

            bondMapping.Add(t, str.ToString());
        }

        private string GenerateStructDeclaration(Type t)
        {
            //This does not handle generics
            if(t.IsGenericType)
                return string.Format("struct {0}<{1}>", t.Name.Replace("`1", ""), t.GenericTypeArguments.Single().Name);
            else if(t.BaseType == typeof(System.Object))
                return string.Format("struct {0}", t.Name);
            else
            {
                //Add newly found type
                GenerateBondStructs(t.BaseType);
                
                return string.Format("struct {0}: {1}", t.Name, t.BaseType.Name);
            }

        }

        private IEnumerable<string> GenerateStructMembers(Type t)
        {
            List<string> members = new List<string>();

            int index = 0;
            foreach (var p in t.GetProperties(System.Reflection.BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead && x.CanWrite))
                //.Where(x => Attribute.IsDefined(x, typeof(DataMemberAttribute))))
            {
                members.Add(GenerateBondProperty(p, index));
                index++;
            }

            return members;
        }

        private string GenerateBondProperty(PropertyInfo info, int location)
        {
            string type = GetBondPropertyType(info.PropertyType);

            return string.Format("{2}: optional nullable<{0}> {1} = nothing;", type, info.Name, location);
        }

        private string GetBondPropertyType(Type t)
        {
            //Collection types vector<T>, map<K, V>, list<T> and set<T> are represented by 
            //respective generic collection interfaces: IList<T>, IDictionary<K, V>, ICollection<T> and ISet<T>.

            string type = "";

            try
            {

                if (t == typeof(string)
                    || t.BaseType == typeof(Enum)
                    || t == typeof(Guid)
                    || t == typeof(Uri))
                    type = "string";
                else if (t == typeof(Int64)
                    || t == typeof(long))
                    type = "int64";
                else if (t == typeof(DateTime))
                    type = "DateTime";
                else if (t == typeof(Int16)
                    || t == typeof(int))
                    type = "int16";
                else if (t == typeof(double)
                    || t == typeof(float)
                    || t == typeof(decimal))
                    type = "double";
                else if (t == typeof(bool))
                    type = "bool";
                else if (t.IsGenericType &&
                        t.GetGenericTypeDefinition() == typeof(ISet<>))
                {
                    type = GenerateBondSet(t);
                }
                else if (t.IsGenericType &&
                    t.GetInterface(typeof(IDictionary<,>).FullName) != null)
                {
                    type = GenerateBondMap(t);
                }
                else if (t.IsGenericType &&
                        (t.GetInterface(typeof(System.Collections.IEnumerable).FullName) != null || t.GetInterface(typeof(System.Collections.ICollection).FullName) != null)
                        || t.IsArray)
                {
                    type = GenerateBondVector(t);
                }
                else if (t.Name == typeof(Nullable<>).Name)
                {
                    type = GetBondPropertyType(t.GetGenericArguments().First());
                }
                else //class
                {
                    GenerateBondStructs(t);
                    type = t.Name;
                }
            }
            catch(Exception)
            {
            }

            return type;
        }

        private string GenerateBondSet(Type t)
        {
            StringBuilder str = new StringBuilder();

            str.Append("set<");

            str.Append(GetBondPropertyType(t.GenericTypeArguments.Single()));

            str.Append(">");

            return str.ToString();
        }

        private string GenerateBondMap(Type t)
        {
            StringBuilder str = new StringBuilder();

            str.Append("map<");

            str.Append(GetBondPropertyType(t.GenericTypeArguments.First()));
            str.Append(",");
            str.Append(GetBondPropertyType(t.GenericTypeArguments.Last()));

            str.Append(">");

            return str.ToString();
        }

        private string GenerateBondVector(Type t)
        {
            //Bond only 
            StringBuilder str = new StringBuilder();

            str.Append("vector<");

            if (t.IsArray)
                str.Append(GetBondPropertyType(t.GetElementType()));
            else
                str.Append(GetBondPropertyType(t.GenericTypeArguments.Single()));

            str.Append(">");

            return str.ToString();
        }
    }
}
