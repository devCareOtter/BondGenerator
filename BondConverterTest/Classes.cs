using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BondConverterTest
{
    [DataContract]
    public class Foo
    {
        [DataMember]
        public string F1 { get; set; }

        [DataMember]
        public bool F2 { get; set; }

        [DataMember]
        public int F3 { get; set; }

        [DataMember]
        public double F4 { get; set; }

        [DataMember]
        public long F5 { get; set; }
    }

    [DataContract]
    public class Bar
    {
        [DataMember]
        public string B1 { get; set; }

        [DataMember]
        public Foo F { get; set; }
    }

    [DataContract]
    public enum testEnum
    {
        [EnumMember]
        def = 0,
        [EnumMember]
        non = 1,
        [EnumMember]
        eck = 2
    }

    [DataContract]
    public class FooNum
    {
        [DataMember]
        public testEnum testE { get; set; }
    }

    [DataContract]
    public class FooBar : Bar
    {
        [DataMember]
        public int Fb { get; set; }
    }

    [DataContract]
    public class FooL
    {
        [DataMember]
        public List<int> data { get; set; }
        
        [DataMember]
        public IEnumerable<Foo> Foos { get; set; }

        [DataMember]
        public Foo[] FoosArray { get; set; }
    }

    [DataContract]
    public class Noo
    {
        public DateTime? data { get; set; }
    }

    public class Doo
    {
        public Dictionary<string, Foo> data { get; set; }
    }

    public class Gen<T>
    {
        public T data { get; set; }
    }

    public class PGen: Gen<String>
    {

    }
}
