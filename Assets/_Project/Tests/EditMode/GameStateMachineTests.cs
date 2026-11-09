using Heartbeat.Core;
using NUnit.Framework;

namespace Heartbeat.Tests
{
    public class GameStateMachineTests
    {
        private static GameStateMachine ToPlaying()
        {
            var sm = new GameStateMachine();
            Assert.IsTrue(sm.TryTransition(GameStateId.MainMenu));
            Assert.IsTrue(sm.TryTransition(GameStateId.Loading));
            Assert.IsTrue(sm.TryTransition(GameStateId.Playing));
            return sm;
        }

        [Test]
        public void NewGame_GoesThroughIntroThenLoading()
        {
            var sm = new GameStateMachine();
            Assert.IsTrue(sm.TryTransition(GameStateId.MainMenu));
            Assert.IsTrue(sm.TryTransition(GameStateId.Intro));
            Assert.IsTrue(sm.TryTransition(GameStateId.Loading));
            Assert.IsTrue(sm.TryTransition(GameStateId.Playing));
            Assert.AreEqual(PlaySubState.Exploration, sm.SubState);
        }

        [Test]
        public void MainMenu_CannotJumpToPlaying()
        {
            var sm = new GameStateMachine();
            sm.TryTransition(GameStateId.MainMenu);
            Assert.IsFalse(sm.TryTransition(GameStateId.Playing));
            Assert.AreEqual(GameStateId.MainMenu, sm.Current);
        }

        [Test]
        public void Pause_ResumesToSameSubState()
        {
            var sm = ToPlaying();
            Assert.IsTrue(sm.TrySetSubState(PlaySubState.Alert));
            Assert.IsTrue(sm.TryTransition(GameStateId.Paused));
            Assert.IsTrue(sm.TryResume());
            Assert.AreEqual(GameStateId.Playing, sm.Current);
            Assert.AreEqual(PlaySubState.Alert, sm.SubState);
        }

        [Test]
        public void PauseInIntro_ResumesToIntro_NotPlaying()
        {
            var sm = new GameStateMachine();
            sm.TryTransition(GameStateId.MainMenu);
            sm.TryTransition(GameStateId.Intro);
            Assert.IsTrue(sm.TryTransition(GameStateId.Paused));
            Assert.IsFalse(sm.TryTransition(GameStateId.Playing));
            Assert.IsTrue(sm.TryResume());
            Assert.AreEqual(GameStateId.Intro, sm.Current);
        }

        [Test]
        public void Restart_FromPause_IsDeath()
        {
            var sm = ToPlaying();
            sm.TryTransition(GameStateId.Paused);
            Assert.IsTrue(sm.TryTransition(GameStateId.Death));
            Assert.IsTrue(sm.TryTransition(GameStateId.Reconstruction));
            Assert.IsTrue(sm.TryTransition(GameStateId.Playing));
        }

        [Test]
        public void Reconstruction_StartsInExploration()
        {
            var sm = ToPlaying();
            sm.TrySetSubState(PlaySubState.Alert);
            sm.TryTransition(GameStateId.Death);
            sm.TryTransition(GameStateId.Reconstruction);
            sm.TryTransition(GameStateId.Playing);
            Assert.AreEqual(PlaySubState.Exploration, sm.SubState);
        }

        [Test]
        public void Surveillance_CannotGoStraightToHacking()
        {
            var sm = ToPlaying();
            Assert.IsTrue(sm.TrySetSubState(PlaySubState.Surveillance));
            Assert.IsFalse(sm.TrySetSubState(PlaySubState.Hacking));
            Assert.IsTrue(sm.TrySetSubState(PlaySubState.Alert));
        }

        [Test]
        public void SubState_RequiresPlaying()
        {
            var sm = new GameStateMachine();
            Assert.IsFalse(sm.TrySetSubState(PlaySubState.Surveillance));
        }

        [Test]
        public void Victory_WithSixDocuments_GoesToDecision()
        {
            var sm = ToPlaying();
            Assert.IsTrue(sm.TryTransition(GameStateId.Victory));
            Assert.IsTrue(sm.TryTransition(GameStateId.FinalDecision));
            Assert.IsTrue(sm.TryTransition(GameStateId.EndingAmbiguous));
            Assert.IsTrue(sm.TryTransition(GameStateId.Credits));
            Assert.IsTrue(sm.TryTransition(GameStateId.MainMenu));
        }

        [Test]
        public void FinalDecision_CanPauseResumeAndQuit() // D-92
        {
            var sm = ToPlaying();
            sm.TryTransition(GameStateId.Victory);
            sm.TryTransition(GameStateId.FinalDecision);
            Assert.IsTrue(sm.TryTransition(GameStateId.Paused));
            Assert.IsFalse(sm.TryTransition(GameStateId.Playing));
            Assert.IsTrue(sm.TryResume());
            Assert.AreEqual(GameStateId.FinalDecision, sm.Current);
            sm.TryTransition(GameStateId.Paused);
            Assert.IsTrue(sm.TryTransition(GameStateId.MainMenu));
        }

        [Test]
        public void Death_FifthConsecutive_CanGoToAssimilation()
        {
            var sm = ToPlaying();
            sm.TryTransition(GameStateId.Death);
            Assert.IsTrue(sm.TryTransition(GameStateId.EndingAssimilation));
            Assert.IsFalse(sm.TryTransition(GameStateId.MainMenu));
            Assert.IsTrue(sm.TryTransition(GameStateId.Credits));
        }

        [Test]
        public void StateChanged_IsRaisedWithFromAndTo()
        {
            var sm = new GameStateMachine();
            GameStateId seenFrom = GameStateId.Quit, seenTo = GameStateId.Quit;
            sm.StateChanged += (f, t) => { seenFrom = f; seenTo = t; };
            sm.TryTransition(GameStateId.MainMenu);
            Assert.AreEqual(GameStateId.Boot, seenFrom);
            Assert.AreEqual(GameStateId.MainMenu, seenTo);
        }
    }
}
