using Entities;
using System;
using System.Reflection;

namespace Server
{
    public class HubInvoker
    {
        public void MethodInvoke(int senderId, Message message)
        {
            try
            {
                var assembly = Assembly.GetCallingAssembly();
                Type type = assembly.GetType(assembly.GetName().Name + "." + message.Hub, false, false);
                var obj = Activator.CreateInstance(type);

                MethodInfo method = obj.GetType().GetMethod(message.Method);
                method.Invoke(obj, new object[] { senderId, message.Data });
            }
            catch
            {

            }
        }
    }
}
