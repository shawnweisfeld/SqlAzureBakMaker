using PowerArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlAzureBakMaker
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class MyArgs
    {
        [ArgRequired(PromptIfMissing = true)]
        public string SourceServer { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string SourceUser { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string SourcePassword { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string SourceDatabase { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string DestinationServer { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string DestinationUser { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string DestinationPassword { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string DestinationDatabase { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string StorageAccountName { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string StorageContainer { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string StorageFileBase { get; set; }

        [ArgRequired(PromptIfMissing = true)]
        public string StorageKey { get; set; }

    }
}
