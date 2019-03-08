using System;

namespace Labyrinth.GameObjects
    {
    public class MovementOfAnother : MovementChecker
        {
        public MovementOfAnother(IMovingItem byWhom) : base(byWhom)
            {
            //if (byWhom.Capability == ObjectCapability.CannotMoveOthers)
            //    throw new ArgumentException("The specified object is not capable of pushing/moving another object.", nameof(byWhom));
            }

        /// <summary>
        /// Initiates a push or bounce involving this object
        /// </summary>
        /// <param name="gameObject">The object that is potentially being moved</param>
        /// <param name="direction">The direction that the specified object is directing this object</param>
        public void PushOrBounce(IMovingItem gameObject, Direction direction)
            {
            bool canCauseBounceBack = this.Source.Properties.Get(GameObjectProperties.Capability) == ObjectCapability.CanPushOrCauseBounceBack;
            var ps = CanBePushedOrBounced(gameObject, this.Source, direction, canCauseBounceBack);
            switch (ps)
                {
                case PushStatus.Yes:
                    {
                    // ToDo what speed should objects move at when pushing and bouncing
                    gameObject.Move(direction, Constants.BaseSpeed * 2);
                    return;
                    }

                case PushStatus.Bounce:
                    {
                    var reverseDirection = direction.Reversed();
                    gameObject.Move(reverseDirection, Constants.BounceBackSpeed);
                    this.Source.BounceBack(reverseDirection, Constants.BounceBackSpeed);
                    this.Source.PlaySound(GameSound.BoulderBounces);
                    return;
                    }

                case PushStatus.No:
                    {
                    return;
                    }

                default:
                    throw new InvalidOperationException();
                }
            }


        }
    }
