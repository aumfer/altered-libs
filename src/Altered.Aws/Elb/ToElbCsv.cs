using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reactive.Linq;

namespace Altered.Aws.Elb
{
    public static class ToCsv
    {
        public static IObservable<CsvReader> ToElbCsv(this IObservable<Stream> streams) =>
            from stream in streams
            select new CsvReader(new StreamReader(stream), new CsvHelper.Configuration.Configuration
            {
                Delimiter = " ",
                HasHeaderRecord = false
            });
    }
}
