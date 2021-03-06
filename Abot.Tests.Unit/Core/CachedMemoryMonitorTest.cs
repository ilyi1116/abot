﻿using Abot.Core;
using Moq;
using NUnit.Framework;
using System;

namespace Abot.Tests.Unit.Core
{
    [TestFixture]
    public class CachedMemoryMonitorTest
    {
        CachedMemoryMonitor _uut;
        Mock<IMemoryMonitor> _fakeMemoryMonitor;

        [SetUp]
        public void Setup()
        {
            _fakeMemoryMonitor = new Mock<IMemoryMonitor>();
        }

        [TearDown]
        public void TearDown()
        {
            if(_uut != null)
                _uut.Dispose();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_MemoryMonitorIsNull()
        {
            new CachedMemoryMonitor(null, 1);
        }

        [Test]
        public void Constructor_CacheExpirationIsZero_CallsMemoryMonitor()
        {
            new CachedMemoryMonitor(_fakeMemoryMonitor.Object, 0);

            _fakeMemoryMonitor.Verify(f => f.GetCurrentUsageInMb(), Times.Exactly(1));
        }

        [Test]
        public void Constructor_CacheExpirationIsNegative_CallsMemoryMonitor()
        {
            new CachedMemoryMonitor(_fakeMemoryMonitor.Object, -1);

            _fakeMemoryMonitor.Verify(f => f.GetCurrentUsageInMb(), Times.Exactly(1));
        }

        [Test]
        public void Constructor_CacheExpirationIsValid_CallsMemoryMonitorEveryCacheExpirationInterval()
        {
            _uut = new CachedMemoryMonitor(_fakeMemoryMonitor.Object, 1);

            _fakeMemoryMonitor.Verify(f => f.GetCurrentUsageInMb(), Times.Exactly(1));

            System.Threading.Thread.Sleep(1200);

            _fakeMemoryMonitor.Verify(f => f.GetCurrentUsageInMb(), Times.Exactly(2));
        }

        [Test]
        public void GetCurrentUsageInMb_SecondCall_DoesNotCallsMemoryMonitorSinceValueWasCached()
        {
            _uut = new CachedMemoryMonitor(_fakeMemoryMonitor.Object, 1);

            _uut.GetCurrentUsageInMb();

            _fakeMemoryMonitor.Verify(f => f.GetCurrentUsageInMb(), Times.Exactly(1));
        }
    }
}
