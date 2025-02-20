using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GeoJSON.Text.Feature;
using GeoJSON.Text.Geometry;
using NUnit.Framework;

namespace GeoJSON.Text.Tests.Feature;

[TestFixture]
public class FeatureCollectionTests : TestBase
{
    [Test]
    public void Ctor_Throws_ArgumentNullException_When_Features_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var featureCollection = new FeatureCollection(null);
        });
    }

    [Test]
    public void Can_Deserialize()
    {
        string json = GetExpectedJson();

        var featureCollection = JsonSerializer.Deserialize<FeatureCollection>(json);

        Assert.IsNotNull(featureCollection);
        Assert.IsNotNull(featureCollection.Features);
        Assert.AreEqual(featureCollection.Features.Count, 3);
        Assert.AreEqual(featureCollection.Features.Count(x => x.Geometry.Type == GeoJSONObjectType.Point), 1);
        Assert.AreEqual(featureCollection.Features.Count(x => x.Geometry.Type == GeoJSONObjectType.MultiPolygon), 1);
        Assert.AreEqual(featureCollection.Features.Count(x => x.Geometry.Type == GeoJSONObjectType.Polygon), 1);
    }

    [Test]
    public void Can_DeserializeGeneric()
    {
        string json = GetExpectedJson();

        var featureCollection =
            JsonSerializer.Deserialize<FeatureCollection<FeatureCollectionTestPropertyObject>>(json);

        Assert.IsNotNull(featureCollection);
        Assert.IsNotNull(featureCollection.Features);
        Assert.AreEqual(featureCollection.Features.Count, 3);
        Assert.AreEqual("DD", featureCollection.Features.First().Properties.name);
        Assert.AreEqual(123, featureCollection.Features.First().Properties.size);
    }

    [Test]
    public void FeatureCollectionSerialization()
    {
        var model = new FeatureCollection();
        for (var i = 10; i-- > 0;)
        {
            var geom = new LineString(new[]
            {
                new Position(51.010, -1.034),
                new Position(51.010, -0.034)
            });

            var props = new Dictionary<string, object>
            {
                { "test1", "1" },
                { "test2", 2 }
            };

            var feature = new Text.Feature.Feature(geom, props);
            model.Features.Add(feature);
        }

        var actualJson = JsonSerializer.Serialize(model);

        Assert.IsNotNull(actualJson);

        Assert.IsFalse(string.IsNullOrEmpty(actualJson));
    }

    [Test]
    public void FeatureCollection_Equals_GetHashCode_Contract()
    {
        var left = GetFeatureCollection();
        var right = GetFeatureCollection();

        Assert_Are_Equal(left, right);
    }

    [Test]
    public void Serialized_And_Deserialized_FeatureCollection_Equals_And_Share_HashCode()
    {
        var leftFc = GetFeatureCollection();
        var leftJson = JsonSerializer.Serialize(leftFc);
        var left = JsonSerializer.Deserialize<FeatureCollection>(leftJson);

        var rightFc = GetFeatureCollection();
        var rightJson = JsonSerializer.Serialize(rightFc);
        var right = JsonSerializer.Deserialize<FeatureCollection>(rightJson);

        Assert_Are_Equal(left, right);
    }

    [Test]
    public void FeatureCollection_Test_IndexOf()
    {
        var model = new FeatureCollection();
        var expectedIds = new List<string>();
        var expectedIndexes = new List<int>();

        for (var i = 0; i < 10; i++)
        {
            var id = "id" + i;

            expectedIds.Add(id);
            expectedIndexes.Add(i);

            var geom = new LineString(new[]
            {
                new Position(51.010, -1.034),
                new Position(51.010, -0.034)
            });

            var props = FeatureTests.GetPropertiesInRandomOrder();

            var feature = new Text.Feature.Feature(geom, props, id);
            model.Features.Add(feature);
        }

        for (var i = 0; i < 10; i++)
        {
            var actualFeature = model.Features[i];
            var actualId = actualFeature.Id;
            var actualIndex = model.Features.IndexOf(actualFeature);

            var expectedId = expectedIds[i];
            var expectedIndex = expectedIndexes[i];

            Assert.AreEqual(expectedId, actualId);
            Assert.AreEqual(expectedIndex, actualIndex);

            Assert.Inconclusive("not supported. the Feature.Id is optional. " +
                                " create a new class that inherits from" +
                                " Feature and then override Equals and GetHashCode");
        }
    }


    private FeatureCollection GetFeatureCollection()
    {
        var model = new FeatureCollection();
        for (var i = 10; i-- > 0;)
        {
            var geom = new LineString(new[]
            {
                new Position(51.010, -1.034),
                new Position(51.010, -0.034)
            });

            var props = FeatureTests.GetPropertiesInRandomOrder();

            var feature = new Text.Feature.Feature(geom, props);
            model.Features.Add(feature);
        }

        return model;
    }

    private void Assert_Are_Equal(FeatureCollection left, FeatureCollection right)
    {
        Assert.AreEqual(left, right);

        Assert.IsTrue(left.Equals(right));
        Assert.IsTrue(right.Equals(left));

        Assert.IsTrue(left.Equals(left));
        Assert.IsTrue(right.Equals(right));

        Assert.IsTrue(left == right);
        Assert.IsTrue(right == left);

        Assert.IsFalse(left != right);
        Assert.IsFalse(right != left);

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
    }

    private class FeatureCollectionTestPropertyObject
    {
        public string name { get; set; }
        public int size { get; set; }
    }
}