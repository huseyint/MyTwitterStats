using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Amazon.S3;
using Amazon.S3.Model;
using MyTwitterStats.Json;
using MyTwitterStats.Models;
using Newtonsoft.Json;

namespace MyTwitterStats.Controllers
{
	public class HomeController : Controller
	{
		private const string AwsAccessKey = "AKIAIS55CGM5PDAFF7QA";
		private const string AwsSecretAccessKey = "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";

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult UploadArchive(HttpPostedFileBase file, int timeZoneOffset)
		{
			var settings = new JsonSerializerSettings { ContractResolver = new UnderscoreMappingResolver() };
			settings.Converters.Add(new TwitterDateConverter(timeZoneOffset));

			var serializer = JsonSerializer.Create(settings);

			var tweetsRaw = new List<Tweet>();

			using (var zipArchive = new ZipArchive(file.InputStream))
			{
				foreach (var entry in zipArchive.Entries.Where(e => Regex.Match(e.FullName, @"data/js/tweets/\d{4}_\d{2}\.js").Success))
				{
					using (var stream = entry.Open())
					using (var streamReader = new StreamReader(stream))
					{
						streamReader.ReadLine();

						var tweets = (Tweet[])serializer.Deserialize(streamReader, typeof(Tweet[]));

						tweetsRaw.AddRange(tweets);
					}
				}
			}

			var stats = Stats.GenerateStats(tweetsRaw);
			var id = SaveStats(stats);

			return RedirectToAction("viewstats", new { Id = id });
		}

		private string SaveStats(Stats stats)
		{
			var id = Guid.NewGuid().ToString("N");

			var stream = new MemoryStream();

			using (var zipStream = new GZipStream(stream, CompressionMode.Compress, true))
			using (var streamWriter = new StreamWriter(zipStream))
			{
				var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings());
				jsonSerializer.Serialize(streamWriter, stats);
			}

			stream.Position = 0;

			using (stream)
			using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretAccessKey))
			{
				var request = new PutObjectRequest();
				request.WithBucketName("MyTwitterStats")
					.WithCannedACL(S3CannedACL.PublicRead)
					.WithKey(id + ".json.gz").InputStream = stream;
				client.PutObject(request);
			}

			return id;
		}

		public ActionResult ViewStats(string id)
		{
			using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(AwsAccessKey, AwsSecretAccessKey))
			{
				var request = new GetObjectRequest();
				request.WithBucketName("MyTwitterStats");
				request.WithKey(id + ".json.gz");

				GetObjectResponse response = null;

				try
				{
					response = client.GetObject(request);
				}
				catch (AmazonS3Exception)
				{
					//TODO: Log exception.ErrorCode
					return HttpNotFound();
				}

				using (response)
				using (var stream = response.ResponseStream)
				using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
				using (var reader = new StreamReader(zipStream))
				{
					var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings());
					var stats = jsonSerializer.Deserialize<Stats>(new JsonTextReader(reader));

					return View(stats);
				}
			}
		}

		public ActionResult About()
		{
			return View();
		}
	}
}