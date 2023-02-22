using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;

namespace UnityVOSample
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();

            // 同じ型に対して、複数の型情報を登録するため名前付きにする
            container.RegisterType<IValueObjectBuilder, ValueObjectDriver1>(nameof(ValueObjectDriver1));
            container.RegisterType<IValueObjectBuilder, ValueObjectDriver2>(nameof(ValueObjectDriver2));

            // 実際に使用するValueObjectクラスと対応付けできるよう関数を作成
            void ValueObjectBuilderRegisterType(Type type, string name)
            {
                var injection = 
                    new InjectionConstructor(new ResolvedParameter<IValueObjectBuilder>(name));
                container.RegisterType(type, injection);
                // InjectionConstructor のコンストラクタを実行する
                container.Resolve(type, null);
            }

            ValueObjectBuilderRegisterType(typeof(VCode1), nameof(ValueObjectDriver1));
            ValueObjectBuilderRegisterType(typeof(VCode2), nameof(ValueObjectDriver2));

            {
                var vo1 = new VCode1("1");
                vo1.Print();
                var vo2 = new VCode2("A");
                vo2.Print();
            }
            {
                var vo1 = new VCode1("2");
                vo1.Print();
                var vo2 = new VCode2("B");
                vo2.Print();
            }
            Console.ReadLine();
        }
    }

    public interface IValueObjectBuilder
    {
        IEnumerable<object> GetData();
    }

    public class CodeName1
    {
        public int code;
        public string name;
    }

    public class ValueObjectDriver1 : IValueObjectBuilder
    {
        public IEnumerable<object> GetData()
        {
            yield return new CodeName1 { code = 1, name = "name1" };
            yield return new CodeName1 { code = 2, name = "name2" };
        }
    }

    public class VCode1
    {
        public string Value { get; }

        private static IValueObjectBuilder _valueObjectBuilder;
        private static Dictionary<int, string> _dic;

        public VCode1(string value)
        {
            Value = value;
            if (_dic == null)
            {
                _dic = _valueObjectBuilder.GetData()
                    .OfType<CodeName1>()
                    .ToDictionary(r => r.code, r => r.name);
                Console.WriteLine("VCode1 Dic作成");
            }
        }
        [InjectionConstructor]
        public VCode1(IValueObjectBuilder valueObjectBuilder)
        {
            _valueObjectBuilder = valueObjectBuilder;
            Console.WriteLine($"VCode1 InjectionConstructor ");
        }
        public void Print()
        {
            Console.WriteLine($"VCode1 Value:{Value}");
            foreach (var m in _dic)
            {
                Console.WriteLine($"code:{m.Key} name:{m.Value}");
            }

        }
    }

    public class CodeName2
    {
        public string code;
        public string name;
    }

    public class ValueObjectDriver2 : IValueObjectBuilder
    {
        public IEnumerable<object> GetData()
        {
            yield return new CodeName2 { code = "a", name = "namea" };
            yield return new CodeName2 { code = "b", name = "nameb" };
        }
    }

    public class VCode2
    {
        public string Value { get; }

        private static IValueObjectBuilder _valueObjectBuilder;
        private static Dictionary<string, string> _dic;

        public VCode2(string value)
        {
            Value = value;
            if (_dic == null)
            {
                _dic = _valueObjectBuilder.GetData()
                    .OfType<CodeName2>()
                    .ToDictionary(r => r.code, r => r.name);
                Console.WriteLine("VCode2 Dic作成");
            }
        }
        [InjectionConstructor]
        public VCode2(IValueObjectBuilder valueObjectBuilder)
        {
            _valueObjectBuilder = valueObjectBuilder;
            Console.WriteLine($"VCode2 InjectionConstructor ");
        }
        public void Print()
        {
            Console.WriteLine($"VCode2 Value:{Value}");
            foreach (var m in _dic)
            {
                Console.WriteLine($"code:{m.Key} name:{m.Value}");
            }

        }
    }
}
