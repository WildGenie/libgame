//
// PluginManagerTests.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2016 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Libgame.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using FileFormat;
    using Libgame.FileFormat;
    using Mono.Addins;
    using NUnit.Framework;

    [TestFixture]
    public class PluginManagerTests
    {
        [Test]
        public void InstanceInitializeAddinManager()
        {
            PluginManager.Shutdown();
            Assert.IsFalse(AddinManager.IsInitialized);
            Assert.IsNotNull(PluginManager.Instance);
            Assert.IsTrue(AddinManager.IsInitialized);
        }

        [Test]
        public void AddinFolderIsHiddenAndExists()
        {
            Assert.IsNotNull(PluginManager.Instance);
            DirectoryInfo dirInfo = new DirectoryInfo(".addins");
            Assert.IsTrue(dirInfo.Exists);
            Assert.IsTrue(dirInfo.Attributes.HasFlag(FileAttributes.Hidden));
        }

        [Test]
        public void ShutdownTurnOffAddinManager()
        {
            Assert.IsNotNull(PluginManager.Instance);
            Assert.IsTrue(AddinManager.IsInitialized);
            PluginManager.Shutdown();
            Assert.IsFalse(AddinManager.IsInitialized);
        }

        [Test]
        public void FindExtensionByGenericType()
        {
            var extensions = PluginManager.Instance
                .FindExtensions<Format>()
                .ToList();
            Assert.IsNotEmpty(extensions);
            Assert.Contains(typeof(StringFormatTest), extensions);
        }

        [Test]
        public void FindSpecificExtensionByGenericTypeFails()
        {
            var extensions = PluginManager.Instance
                                          .FindExtensions<StringFormatTest>();
            Assert.IsEmpty(extensions);
        }

        [Test]
        public void FindExtension()
        {
            var extensions = PluginManager.Instance
                                          .FindExtensions(typeof(Format))
                                          .ToList();
            Assert.IsNotEmpty(extensions);
            Assert.Contains(typeof(StringFormatTest), extensions);
        }

        [Test]
        public void FindExtensionNotRegisteredReturnsEmpty()
        {
            var extensions = PluginManager.Instance
                                          .FindExtensions(typeof(StringFormatTest));
            Assert.IsEmpty(extensions);
        }

        [Test]
        public void FindExtensionWithNullTypeThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                                             PluginManager.Instance.FindExtensions(null));
        }

        [Test]
        public void FindGenericExtensions()
        {
            var extensions = PluginManager.Instance
                .FindGenericExtensions(typeof(IConverter<,>), typeof(string), typeof(uint))
                .ToList();
            Assert.IsNotEmpty(extensions);
            Assert.Contains(typeof(SingleOuterConverterExample), extensions);
        }

        [Test]
        public void FindGenericExtensionsNotRegisteredReturnsEmpty()
        {
            var extensions = PluginManager.Instance
                .FindGenericExtensions(typeof(DummyExtensionPoint<>), typeof(uint))
                .ToList();
            Assert.IsEmpty(extensions);
        }

        [Test]
        public void FindGenericExtensionWithInvalidTypeArgumentsReturnEmpty()
        {
            var extensions = PluginManager.Instance
                .FindGenericExtensions(typeof(IConverter<,>), typeof(string), typeof(string))
                .ToList();
            Assert.IsEmpty(extensions);
        }

        [Test]
        public void FindGenericExtensionWithDerivedTypeArgumentsDoesNotReturnEmpty()
        {
            var extensions = PluginManager.Instance
                .FindGenericExtensions(typeof(IConverter<,>), typeof(Format), typeof(int))
                .ToList();
            Assert.IsNotEmpty(extensions);
            Assert.Contains(typeof(StringFormatTest2IntConverter), extensions);
        }

        [Test]
        public void FindGenericExtensionWithNullTypeThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                                 PluginManager.Instance.FindGenericExtensions(null));
        }

        [Test]
        public void FindGenericExtensionWithNEmptyTypeArgumentReturnsEmpty()
        {
            var extensions = PluginManager.Instance
                .FindGenericExtensions(typeof(IConverter<,>))
                .ToList();
            Assert.IsEmpty(extensions);
        }

        [Test]
        public void FindGenericExtensionWithNullTypeArgumentReturnsEmpty()
        {
            var extensions = PluginManager.Instance
                .FindGenericExtensions(typeof(IConverter<,>), null, null)
                .ToList();
            Assert.IsEmpty(extensions);
        }

        public interface DummyExtensionPoint<T>
        {
        }
    }
}