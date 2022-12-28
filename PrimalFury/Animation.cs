using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Window;

namespace PrimalFury {

    public delegate T Animate<T>(float phase);

    public enum AnimationState{   
        None,
        Running,
        Freezed,
        Stopped,
        Executed
    }

    public class Animation<T>{

        Animate<T> _anim;
        long _duration;
        bool _repeatable;
        bool _reversable;

        Clock _counter;
        AnimationState _state;

        long _shift;
        T _lastKnownState;

        public AnimationState State { get { return _state; } }

        public long InternalCounter { get { return _counter.ElapsedTime.AsMilliseconds(); } }
        public long Duration { get { return _duration; } set { _duration = value; } }

        // Function of animation phase (0 to 1) returning 2x2 matrix
        // Duration of FULL REPEATING CYCLE (including reverse part, if exists) - milliseconds
        public Animation(Animate<T> anim, long duration, bool repeatable = true, bool reversable = false, float startShift = 0) {
            _anim = anim;
            _duration = duration;
            _repeatable = repeatable;
            _reversable = reversable;
            _shift = (long)Math.Round(startShift*duration);
        }

        public void Start() {

            if (_state != AnimationState.Running) {
                _counter = new Clock();
                Console.WriteLine(_counter.ElapsedTime.AsMilliseconds());
                _state = AnimationState.Running;
            }
        }

        public void Freeze() {

            if (_state == AnimationState.Running) {
                _state = AnimationState.Freezed;
                _shift = (_shift + _counter.ElapsedTime.AsMilliseconds())  % _duration;
                Console.WriteLine(_shift);
                _counter.Dispose();
            }
        }

        public void Stop() {

            if (_state != AnimationState.Stopped&& _state != AnimationState.None) {
                _state = AnimationState.Stopped;
                _shift = 0;
                _counter.Dispose();
            }
        }

        public T Get() {
            if (_state == AnimationState.None) {
                throw new InvalidOperationException("Attempt to use Get() before first Start()"); 
            }
            if (_state == AnimationState.Running) {
                var probe = _counter.ElapsedTime.AsMilliseconds();
                long x = (_shift + probe) % _duration;
                if (!_repeatable && probe >= _duration) {
                    Stop();
                    _lastKnownState = _anim.Invoke(_reversable ? 0 : 1);
                    return _lastKnownState;
                }

                float arg = (float)(_reversable ? _duration / 2 - Math.Abs(x - _duration / 2) : x) / ((float)_duration / (_reversable  ? 2 : 1));
                var transf = _anim.Invoke(arg);


                _lastKnownState = transf;
                return transf;
            }

            return _lastKnownState;
        }
    }
}
