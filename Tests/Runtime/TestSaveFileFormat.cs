using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using static Buck.SaveAsync.Tests.AsyncToCoroutine;

namespace Buck.SaveAsync.Tests
{
    internal class TestSaveObject
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }
    }
    
    /// <summary>
    /// These tests verify the json format of save files generated by the save system. Useful to detect when a change
    /// is introduced which may break existing saves.
    /// </summary>
    public class TestSaveFileFormat : TestCaseBase
    {
        [UnityTest]
        public IEnumerator Test_SaveFormat_NestedDictionary() => AsCoroutine(async () => 
        {
            // Arrange
            var nestedObject = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", 2 },
                {
                    "key3", new Dictionary<string, object>
                    {
                        { "key4", "value4" },
                        { "key5", 5 }
                    }
                }
            };
            
            // Act
            var key = Guid.NewGuid().ToString();
            var serializedFile = await GetSerializedFileForObject(key, nestedObject);
            
            // Assert
            var expected = $@"
[
  {{
    ""Key"": ""{key}"",
    ""Data"": {{
      ""$type"": ""System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Object, mscorlib]], mscorlib"",
      ""key1"": ""value1"",
      ""key2"": 2,
      ""key3"": {{
        ""$type"": ""System.Collections.Generic.Dictionary`2[[System.String, mscorlib],[System.Object, mscorlib]], mscorlib"",
        ""key4"": ""value4"",
        ""key5"": 5
      }}
    }}
  }}
]
";
            MultilineDiffUtils.AssertMultilineStringEqual(expected,serializedFile);
        });
        
        [UnityTest]
        public IEnumerator Test_SaveFormat_BasicObject() => AsCoroutine(async () => 
        {
            // Arrange
            var nestedObject = new TestSaveObject
            {
                IntValue = 1337,
                StringValue = "Goodbye, World!"
            };
            
            // Act
            var key = Guid.NewGuid().ToString();
            var serializedFile = await GetSerializedFileForObject(key, nestedObject);
            
            // Assert
            var expected = $@"
[
  {{
    ""Key"": ""{key}"",
    ""Data"": {{
      ""$type"": ""{TestConstants.Namespace}.TestSaveObject, {TestConstants.Assembly}"",
      ""IntValue"": 1337,
      ""StringValue"": ""Goodbye, World!""
    }}
  }}
]
";
            MultilineDiffUtils.AssertMultilineStringEqual(expected,serializedFile);
        });
    }
}