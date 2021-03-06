﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Nest;
using Newtonsoft.Json.Converters;
using Nest.Resolvers.Converters;
using Nest.TestData.Domain;

namespace Nest.Tests.Integration
{
	[TestFixture]
	public class HighlightIntegrationTests : BaseElasticSearchTests
	{
		[Test]
		public void TestHighlight()
		{
      var result = this.ConnectedClient.Search<ElasticSearchProject>(s => s
        .From(0)
        .Size(10)
        .Query(q=>q
          .QueryString(qs=>qs
            .OnField(e=>e.Content)
            .Query("null or null*")
          )
        )
        .Highlight(h => h
          .PreTags("<b>")
          .PostTags("</b>")
          .OnFields(
            f => f
              .OnField(e=>e.Content)
              .PreTags("<em>")
              .PostTags("</em>")
          )
        )
      );
      Assert.IsTrue(result.IsValid);
      Assert.DoesNotThrow(() => result.Highlights.Count());
      Assert.IsNotNull(result.Highlights);
      Assert.Greater(result.Highlights.Count(), 0);
      Assert.True(result.Highlights.Any(h => h.Highlights != null && h.Highlights.Any() && !string.IsNullOrEmpty(h.Highlights.First())));
		}

    [Test]
    public void TestHighlightNoNullRef()
    {
      var result = this.ConnectedClient.Search<ElasticSearchProject>(s => s
        .From(0)
        .Size(10)
        .Query(q => q
          .QueryString(qs => qs
            .Query("elasticsearch.pm")
          )
        )
        .Highlight(h => h
          .PreTags("<b>")
          .PostTags("</b>")
          .OnFields(
            f => f
              .OnField(e => e.Content)
              .PreTags("<em>")
              .PostTags("</em>")
          )
        )
      );
      Assert.IsTrue(result.IsValid);
      Assert.DoesNotThrow(() => result.Highlights.Count());
      Assert.IsNotNull(result.Highlights);
      Assert.GreaterOrEqual(result.Total, 1);
      Assert.AreEqual(result.Highlights.Count(), 0);
    }
  }
}
