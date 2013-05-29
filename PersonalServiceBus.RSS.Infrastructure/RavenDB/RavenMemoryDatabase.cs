using PersonalServiceBus.RSS.Core.Domain.Interface;
using Raven.Client.Embedded;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class RavenMemoryDatabase : RavenDatabase
    {
        public RavenMemoryDatabase(IConfiguration configuration)
            :base(configuration)
        {
            DocumentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true
                };
            DocumentStore.Initialize();
        }
    }
}