using System;
using System.Collections.Generic;

namespace Core.Common.Interfaces
{
    public interface ICacheService
    {
        bool IsCacheServiceAlive();
        void AddValueAsString(string pKey, string pObj);
        void UpdateValueWithString(string pKey, string pObj);
        void AddValueAsJson(string pKey, object pObj);
        void UpdateValueWithJson(string pKey, object pObj);
        void ClearItemByKey(string pKey);
        bool KeyExist(string pKey);
        List<dynamic> GetJsonValuesByKey(string pKey, Type pType);
        bool Disconnect();
    }
}
