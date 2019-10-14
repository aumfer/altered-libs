using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using static Amazon.S3.Util.S3EventNotification;

namespace Altered.Aws.S3
{
    public static class GetBucketObjectsExtensions
    {
        public static Task<IEnumerable<GetObjectResponse>> GetBucketObjectsAsync(this IAmazonS3 s3, S3EventNotification s3EventNotification) =>
            s3.GetBucketObjects(Observable.Return(s3EventNotification)).ToList().Cast<IEnumerable<GetObjectResponse>>().ToTask();
        public static IObservable<GetObjectResponse> GetBucketObjects(this IAmazonS3 s3, IObservable<S3EventNotification> s3EventNotifications) =>
            from s3Event in s3EventNotifications
                .ObserveOn(Scheduler.Default)
            from s3Record in s3Event.Records
            let bucketName = s3Record.S3.Bucket.Name
            let objectKey = s3Record.S3.Object.Key
            let getObjectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            }
            from getObjectResponse in s3.GetObjectAsync(getObjectRequest)
            select getObjectResponse;
    }
}
