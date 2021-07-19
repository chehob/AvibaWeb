using Cyriller;
using Cyriller.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.Infrastructure
{
    public interface ICyrillerService
    {
        CyrResult DeclineName(string name);
        CyrResult DeclinePhrase(string phrase);
    }

    public class CyrillerService : ICyrillerService
    {
        private readonly CyrNounCollection cyrNounCollection;
        private readonly CyrAdjectiveCollection cyrAdjectiveCollection;

        public CyrillerService()
        {
            cyrNounCollection = new CyrNounCollection();
            cyrAdjectiveCollection = new CyrAdjectiveCollection();
        }

        public CyrResult DeclineName(string name)
        {
            CyrName cyrName = new CyrName();
            CyrResult result = cyrName.Decline(name);
            cyrName = null;
            return result;
        }

        public CyrResult DeclinePhrase(string phrase)
        {
            CyrPhrase cyrPhrase = new CyrPhrase(cyrNounCollection, cyrAdjectiveCollection);
            CyrResult result = cyrPhrase.Decline(phrase, GetConditionsEnum.Similar);
            cyrPhrase = null;
            return result;
        }
    }
}
