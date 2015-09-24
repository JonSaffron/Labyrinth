using System;
using Labyrinth.Services.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Moq;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestActiveSoundService
        {
        [Test]
        public void GivenThatTheActiveSoundListIsEmptyCanANewSoundWithoutGameObjectBeAddedAndHaveTheDefaultVolumeAndPanning()
            {
            var ass = new ActiveSoundService();
            var sei = new DummySoundEffectInstance();
            var activeSound = new ActiveSound(sei);
            
            ass.Add(activeSound);

            Assert.AreEqual(1, ass.Count);
            var item = ass[0];
            Assert.AreEqual(SoundState.Playing, item.SoundEffectInstance.State);
            Assert.AreEqual(1, item.SoundEffectInstance.Volume);
            Assert.AreEqual(0, item.SoundEffectInstance.Pan);
            }

        [Test]
        public void GivenThatTheActiveSoundListIsEmptyCanANewSoundWithGameObjectBeAddedAndNotHaveTheDefaultVolumeAndPanning()
            {
            var ass = new ActiveSoundService();
            var sei = new DummySoundEffectInstance();
            var gameObject = new Mock<IGameObject>();
            gameObject.Setup(deadGameObject => deadGameObject.IsExtant).Returns(true);
            gameObject.Setup(objectToTheSide => objectToTheSide.Position).Returns(new Vector2(10, 10));
            var centrePointProvider = new Mock<ICentrePointProvider>();
            centrePointProvider.Setup(cp => cp.CentrePoint).Returns(Vector2.Zero);
            var activeSound = new ActiveSoundForObject(sei, gameObject.Object, centrePointProvider.Object);

            ass.Add(activeSound);

            Assert.AreEqual(1, ass.Count);
            var item = ass[0];
            Assert.AreEqual(SoundState.Playing, item.SoundEffectInstance.State);
            Assert.AreNotEqual(1, item.SoundEffectInstance.Volume);
            Assert.AreNotEqual(0, item.SoundEffectInstance.Pan);
            }

        [Test]
        public void IfAnAttemptIsMadeToAddASoundForANonExtantObjectWillItBeIgnored()
            {
            var ass = new ActiveSoundService();
            var sei = new DummySoundEffectInstance();
            var gameObject = new Mock<IGameObject>();
            gameObject.Setup(deadGameObject => deadGameObject.IsExtant).Returns(false);
            var centrePointProvider = new Mock<ICentrePointProvider>();
            centrePointProvider.Setup(cp => cp.CentrePoint).Returns(Vector2.Zero);
            var activeSound = new ActiveSoundForObject(sei, gameObject.Object, centrePointProvider.Object);

            ass.Add(activeSound);

            Assert.AreEqual(1, ass.Count);
            var item = ass[0];
            Assert.AreEqual(SoundState.Stopped, item.SoundEffectInstance.State);
            }

        //[Test]
        //public void GivenThatTheActiveSoundListIsNotEmptyCanADifferentSoundBeAdded()
        //    {
        //    var ass = new ActiveSoundService();
        //    var sei1 = new Mock<ISoundEffectInstance>();
        //    var sei2 = new Mock<ISoundEffectInstance>();
           
        //    ass.Add(sei1.Object, null);
        //    ass.Add(sei2.Object, null);

        //    Assert.Equals(ass.Count, 2);
        //    }

        //[Test]
        //public void GivenThatTheActiveSoundListContainsAGivenInstanceDoesThatSoundRestartWhenAddedAgain()
        //    {
        //    var ass = new ActiveSoundService();
        //    var sei = new Mock<ISoundEffectInstance>();
            
        //    ass.Add(sei.Object, null);
        //    ass.Add(sei.Object, null);

        //    Assert.Equals(ass.Count, 1);
        //    Assert.IsTrue(ass[0].RestartPlay);
        //    }

        //[Test]
        //public void GivenThatTheActiveSoundListIsNotEmptyDoesClearLeaveItEmpty()
        //    {
        //    var ass = new ActiveSoundService();
        //    var sei = new Mock<ISoundEffectInstance>();
            
        //    ass.Add(sei.Object, null);
        //    ass.Add(sei.Object, null);
        //    ass.Clear();

        //    Assert.Equals(ass.Count, 0);
        //    }

        //[Test]
        //public void GivenTheListContainsACompletedItemDoesUpdateDropThatItem()
        //    {
        //    var ass = new ActiveSoundService();
        //    var sei = new Mock<ISoundEffectInstance>();
        //    sei.Setup(finishedPlaying => finishedPlaying.State).Returns(SoundState.Stopped);
            
        //    ass.Add(sei.Object, null);
        //    ass.Update(Vector2.Zero);

        //    Assert.Equals(ass.Count, 0);
        //    }

        //[Test]
        //public void DoesAPlayingSoundStopIfTheAssociatedObjectIsNoLongerExtant()
        //    {
        //    var ass = new ActiveSoundService();
        //    var sei = new Mock<ISoundEffectInstance>();
        //    var gameObject = new Mock<StaticItem>();
        //    gameObject.SetupSequence(deadGameObject => deadGameObject.IsExtant)
        //        .Returns(true)
        //        .Returns(false);

        //    ass.Add(sei.Object, gameObject.Object);
        //    ass.Update(Vector2.Zero);

        //    Assert.Equals(ass.Count, 0);
        //    }

        //[Test]
        //public void DoesAPlayingSoundChangeItsPanningWhenItsAssociatedObjectMoves()
        //    {
        //    var ass = new ActiveSoundService();
        //    var sei = new Mock<ISoundEffectInstance>();
        //    var gameObject = new Mock<StaticItem>();
        //    gameObject.Setup(movingObject => movingObject.IsExtant).Returns(true);
        //    gameObject.SetupSequence(movingObject => movingObject.Position)
        //        .Returns(new Vector2(-10, 0))
        //        .Returns(new Vector2(10, 0));

        //    var seiInstance = sei.Object;
        //    ass.Add(seiInstance, gameObject.Object);
        //    var initialPanning = seiInstance.Pan;
        //    ass.Update(Vector2.Zero);
        //    var updatedPanning = seiInstance.Pan;

        //    Assert.AreNotEqual(initialPanning, updatedPanning);
        //    }

        //[Test]
        //public void DoesAPlayingSoundChangeItsVolumeWhenItsAssociatedObjectMoves()
        //    {
        //    var ass = new ActiveSoundService();
        //    var sei = new Mock<ISoundEffectInstance>();
        //    var gameObject = new Mock<StaticItem>();
        //    gameObject.Setup(movingObject => movingObject.IsExtant).Returns(true);
        //    gameObject.SetupSequence(movingObject => movingObject.Position)
        //        .Returns(Vector2.Zero)
        //        .Returns(new Vector2(100, 0));

        //    var seiInstance = sei.Object;
        //    ass.Add(seiInstance, gameObject.Object);
        //    var initialVolume = seiInstance.Volume;
        //    ass.Update(Vector2.Zero);
        //    var updatedVolume = seiInstance.Volume;

        //    Assert.AreNotEqual(initialVolume, updatedVolume);
        //    }

        class DummySoundEffectInstance : ISoundEffectInstance
            {
            private SoundState _state = SoundState.Stopped;
            private float _pan;
            private float _volume;
            private string _instanceName = "Instance";

            public DummySoundEffectInstance()
                {
                Volume = 1.0f;
                }

            public void Dispose()
                {
                // nothing to do
                }

            public void Play()
                {
                this._state = SoundState.Playing;
                }

            public void Stop()
                {
                this._state = SoundState.Stopped;
                }

            public string InstanceName
                {
                get
                    {
                    return this._instanceName;
                    }
                set
                    {
                    this._instanceName = value;
                    }
                }

            public SoundState State
                {
                get
                    {
                    return _state;
                    }
                }

            public float Pan
                {
                get
                    {
                    return _pan;
                    }
                set
                    {
                    if (value < -1.0f || value > 1.0f)
                        throw new ArgumentOutOfRangeException();
                    _pan = value;
                    }
                }

            public float Volume
                {
                get
                    {
                    return _volume;
                    }
                set
                    {
                    if (value < 0.0f || value > 1.0f)
                        throw new ArgumentOutOfRangeException();
                    _volume = value;
                    }
                }
            }
        }
    }
