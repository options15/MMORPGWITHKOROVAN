using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Reflection;

namespace Server
{
    class MessageParser
    {
        public void ParseMessage(string data, int id)
        {
            var json = JsonConvert.DeserializeObject<Message>(data);
            var name = AssemblyName.GetAssemblyName(json.Class);
            var obj = Activator.CreateInstance(json.Class, name.Name);

            MethodInfo method = obj.GetType().GetMethod(json.Method);
            method.Invoke(json.Data, null);
        }


    }
}
