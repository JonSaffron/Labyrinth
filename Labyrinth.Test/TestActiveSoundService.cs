using System;
using Labyrinth.Services.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Moq;
using NUnit.Framework;

// ReSharper disable ObjectCreationAsStatement

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
        public void CanAnActiveSoundBeCreatedForDeadObject()
            {
            var sei = new DummySoundEffectInstance();
            var gameObject = new Mock<IGameObject>();
            gameObject.Setup(deadGameObject => deadGameObject.IsExtant).Returns(false);
            var centrePointProvider = new Mock<ICentrePointProvider>();
            centrePointProvider.Setup(cp => cp.CentrePoint).Returns(Vector2.Zero);
            Assert.Throws<ArgumentException>(() => new ActiveSoundForObject(sei, gameObject.Object, centrePointProvider.Object));
            }

        [Test]
        public void GivenThatTheActiveSoundListIsNotEmptyCanADifferentSoundBeAdded()
            {
            var ass = new ActiveSoundService();
            var sei1 = new DummySoundEffectInstance {InstanceName = "one"};
            var sei2 = new DummySoundEffectInstance {InstanceName = "two"};
            var activeSound1 = new ActiveSound(sei1);
            var activeSound2 = new ActiveSound(sei2);

            ass.Add(activeSound1);
            ass.Add(activeSound2);

            Assert.AreEqual(2, ass.Count);
            }

        [Test]
        public void GivenThatTheActiveSoundListContainsAGivenInstanceDoesThatSoundRestartWhenAddedAgain()
            {
            var ass = new ActiveSoundService();
            var sei = new DummySoundEffectInstance();
            var activeSound = new ActiveSound(sei);
            
            ass.Add(activeSound);
            ass.Add(activeSound);

            Assert.AreEqual(1, ass.Count);
            // ReSharper disable once RedundantAssignment
            activeSound = (ActiveSound) ass[0];
            Assert.IsTrue(sei.RestartPlayWhenStopped);
            }

        [Test]
        public void GivenThatTheActiveSoundListPlaysASoundWithAGameObjectWillAddingTheSoundAgainWithoutAGameObjectResetThePanAndVolume()
            {
            var ass = new ActiveSoundService();
            var sei = new DummySoundEffectInstance();
            var gameObject = new Mock<IGameObject>();
            gameObject.Setup(aliveGameObject => aliveGameObject.IsExtant).Returns(true);
            gameObject.Setup(objectToTheSide => objectToTheSide.Position).Returns(new Vector2(10, 10));
            var centrePointProvider = new Mock<ICentrePointProvider>();
            centrePointProvider.Setup(cp => cp.CentrePoint).Returns(Vector2.Zero);
            var activeSoundWithObject = new ActiveSoundForObject(sei, gameObject.Object, centrePointProvider.Object);
            var activeSoundWithoutObject = new ActiveSound(sei);
            
            ass.Add(activeSoundWithObject);
            ass.Add(activeSoundWithoutObject);

            Assert.AreEqual(1, ass.Count);
            activeSoundWithoutObject = (ActiveSound) ass[0];
            Assert.IsTrue(sei.RestartPlayWhenStopped);
            ass.Update();
            Assert.IsFalse(sei.RestartPlayWhenStopped);
            Assert.AreEqual(1, activeSoundWithoutObject.SoundEffectInstance.Volume);
            Assert.AreEqual(0, activeSoundWithoutObject.SoundEffectInstance.Pan);
            }

        [Test]
        public void GivenThatTheActiveSoundListIsNotEmptyDoesClearLeaveItEmpty()
            {
            var ass = new ActiveSoundService();
            var sei1 = new DummySoundEffectInstance { InstanceName = "one" };
            var sei2 = new DummySoundEffectInstance { InstanceName = "two" };
            var activeSound1 = new ActiveSound(sei1);
            var activeSound2 = new ActiveSound(sei2);
            
            ass.Add(activeSound1);
            ass.Add(activeSound2);
            ass.Clear();
            ass.Update();

            Assert.AreEqual(0, ass.Count);
            }

        [Test]
        public void GivenTheListContainsACompletedItemDoesUpdateDropThatItem()
            {
            var ass = new ActiveSoundService();
            var sei = new DummySoundEffectInstance();
            var activeSound = new ActiveSound(sei);
            
            ass.Add(activeSound);
            Assert.AreEqual(1, ass.Count);
            var item = ass[0];
            Assert.AreEqual(SoundState.Playing, item.SoundEffectInstance.State);

            activeSound.Stop();
            ass.Update();

            Assert.AreEqual(ass.Count, 0);
            }

        [Test]
        public void DoesAPlayingSoundStopIfTheAssociatedObjectIsNoLongerExtant()
            {
            var ass = new ActiveSoundService();
            var sei = new DummySoundEffectInstance();

            var gameObject = new Mock<IGameObject>();
            gameObject.Setup(deadGameObject => deadGameObject.IsExtant).Returns(true);
            var centrePointProvider = new Mock<ICentrePointProvider>();
            centrePointProvider.Setup(cp => cp.CentrePoint).Returns(Vector2.Zero);
            var activeSound = new ActiveSoundForObject(sei, gameObject.Object, centrePointProvider.Object);

            ass.Add(activeSound);
            gameObject.Setup(deadGameObject => deadGameObject.IsExtant).Returns(false);
            ass.Update();

            Assert.AreEqual(1, ass.Count);
            var item = ass[0];
            Assert.AreEqual(SoundState.Stopped, item.SoundEffectInstance.State);
            }
        
        [Test]
        public void DoesAPlayingSoundChangeItsPanningWhenItsAssociatedObjectMoves()
            {
            var ass = new ActiveSoundService();
            var sei = new DummySoundEffectInstance();
            var gameObject = new Mock<IGameObject>();
            gameObject.Setup(movingObject => movingObject.IsExtant).Returns(true);
            gameObject.SetupSequence(movingObject => movingObject.Position)
                .Returns(new Vector2(-10, 0))
                .Returns(new Vector2(10, 0));
            var centrePointProvider = new Mock<ICentrePointProvider>();
            centrePointProvider.Setup(cp => cp.CentrePoint).Returns(Vector2.Zero);
            var activeSound = new ActiveSoundForObject(sei, gameObject.Object, centrePointProvider.Object);

            ass.Add(activeSound);
            var initialPanning = sei.Pan;
            ass.Update();
            var updatedPanning = sei.Pan;

            Assert.AreNotEqual(initialPanning, updatedPanning);
            }

        [Test]
        public void DoesAPlayingSoundChangeItsVolumeWhenItsAssociatedObjectMoves()
            {
            var ass = new ActiveSoundService();
            var sei = new DummySoundEffectInstance();
            var gameObject = new Mock<IGameObject>();
            gameObject.Setup(movingObject => movingObject.IsExtant).Returns(true);
            gameObject.SetupSequence(movingObject => movingObject.Position)
                .Returns(Vector2.Zero)
                .Returns(new Vector2(100, 0));
            var centrePointProvider = new Mock<ICentrePointProvider>();
            centrePointProvider.Setup(cp => cp.CentrePoint).Returns(Vector2.Zero);
            var activeSound = new ActiveSoundForObject(sei, gameObject.Object, centrePointProvider.Object);

            ass.Add(activeSound);
            var initialVolume = sei.Volume;
            ass.Update();
            var updatedVolume = sei.Volume;

            Assert.AreNotEqual(initialVolume, updatedVolume);
            }
        }
    }
