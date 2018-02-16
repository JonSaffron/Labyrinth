using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth.GameObjects
    {
    public class MovementOfAnother : MovementChecker
        {
        public MovementOfAnother(IMovingItem byWhom) : base(byWhom)
            {
            if (byWhom.Capability == ObjectCapability.CannotMoveOthers)
                throw new ArgumentException("The specified object is not capable of pushing/moving another object.", nameof(byWhom));
            }

        /// <summary>
        /// Initiates a push or bounce involving this object
        /// </summary>
        /// <param name="direction">The direction that the specified object is directing this object</param>
        public void PushOrBounce(IMovingItem gameObject, Direction direction)
            {
            bool canCauseBounceBack = this._source.Capability == ObjectCapability.CanPushOrCauseBounceBack;
            var ps = CanBePushedOrBounced(gameObject, direction, canCauseBounceBack);
            switch (ps)
                {
                case PushStatus.Yes:
                    {
                    gameObject.Move(direction, gameObject.StandardSpeed);
                    return;
                    }

                case PushStatus.Bounce:
                    {
                    var reverseDirection = direction.Reversed();
                    gameObject.Move(reverseDirection, gameObject.BounceBackSpeed);
                    this._source.BounceBack(reverseDirection, gameObject.BounceBackSpeed);
                    this._source.PlaySound(GameSound.BoulderBounces);
                    return;
                    }

                default:
                    throw new InvalidOperationException();
                }
            }


        }
    }
