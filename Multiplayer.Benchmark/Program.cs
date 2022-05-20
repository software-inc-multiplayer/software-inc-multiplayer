using System;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Multiplayer.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    //[SimpleJob(RuntimeMoniker.CoreRt50)]
    [MemoryDiagnoser]
    public class PacketLookup {
        private const string strCounter = "someRandomTest/";
        private const string strPlaceHolder = "some.More.Strings";
        public Dictionary<int, Type> intLookup = new Dictionary<int, Type>();
        public Dictionary<string, Type> stringLookup = new Dictionary<string, Type>();
        public List<Type> listLookup = new List<Type>();
        public List<string> strValues = new List<string>();

        [GlobalSetup]
        public void Setup() {
            for(var counter = 0; counter < 100; counter++) {
                intLookup.Add(counter, typeof(PacketLookup));
                var strValue = $"{strPlaceHolder}{counter}";
                strValues.Add($"{strCounter}{strValue}");
                stringLookup.Add(strValue, typeof(PacketLookup));
                listLookup.Add(typeof(PacketLookup));
            }

            var rnd = new Random();
            strValues = strValues.OrderBy(item => rnd.Next()).ToList();
        }

        [Benchmark]
        public int LookupByString() {
            var counter = 0;
            foreach(var strValue in strValues) {
                //ReadOnlySpan<char> spanValue = strValue;
                var indexOfSplitter = strValue.IndexOf('/') + 1;
                if (indexOfSplitter == 0)
                    continue;
                var strRealValue = strValue.Substring(indexOfSplitter);
                //var strRealValue = spanValue[indexOfSplitter..].ToString();
                
                if(stringLookup[strRealValue] == typeof(PacketLookup)) {
                    counter++;
                }
            }
            return counter;
        }

        [Benchmark]
        public int LookupByInt() {
            var counter2 = 0;
            for(var counter = 0; counter < 100; counter++) {
                if(intLookup[counter] == typeof(PacketLookup)) {
                    counter2++;
                }
            }
            return counter2;
        }

        [Benchmark]
        public int ListLookup() {
            var counter = 0;
            foreach(var elem in listLookup) {
                if(elem == typeof(PacketLookup)) {
                    counter++;
                }
            }
            return counter;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            /*var x = new PacketLookup();
            x.Setup();
            x.LookupByString();*/
            var summary = BenchmarkRunner.Run<PacketLookup>();

        }
    }
}
