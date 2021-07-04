using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;

namespace Graph.Bayesian.WPF.Models.Vertices.History
{
    public class PositionalModel
    {
        public enum Group
        {
            Post, Ante, Current
        }
    }

    public class PositionalModel<T, TKey> : PositionalModel
       where TKey : notnull
       where T : IEquatable<T>
    {
        private readonly Func<T, TKey> _keyFunc;
        //private readonly Func<T, TR> _createFunc;

        private readonly Stack<T> post = new();
        private readonly Stack<T> ante = new();
        private T? current;

        public PositionalModel(Func<T, TKey> keyFunc) //, Func<T, TR> createFunc)
        {
            _keyFunc = keyFunc;
            //_createFunc = createFunc;
        }

        public IReadOnlyCollection<T> Post => post;

        public IReadOnlyCollection<T> Ante => ante;

        public IReadOnlyCollection<T> currentCollection => current == null ? Array.Empty<T>() : new[] { current };

        public T? Current => current;

        public IReadOnlyCollection<T> Collection => Ante.Concat(new[] { current }).Concat(Post).ToArray();

        public IReadOnlyCollection<TKey> Keys => Ante.Concat(currentCollection).Concat(Post).Select(a => _keyFunc(a)).ToArray();


        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public Change<GroupedItem<T, Group, TKey>, TKey>[] AddToCurrent(T value)
        {
            if (Current != null)
            {
                throw new Exception("Current must be null before setting");
            }

            ante.Clear();
            post.Clear();
            var key = _keyFunc(current = value);
            return new Change<GroupedItem<T, Group, TKey>, TKey>[] {
                new (ChangeReason.Add,key, new(Group.Current,key,  value))
            };
        }

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public Change<GroupedItem<T, Group, TKey>, TKey>[] AddToAnterior(T value)
        {
            ante.Push(current);
            var collect = Collect(value, current).ToArray();           
            post.Clear();
            current = value;
            return collect;


            IEnumerable<Change<GroupedItem<T, Group, TKey>, TKey>> Collect(T value, T current)
            {
                var currentKey = _keyFunc(current ?? throw new Exception("Current can't be null before adding to Anterior"));
                var valueKey = _keyFunc(value);
                // move current to complete
                yield return new(ChangeReason.Add, currentKey, new(Group.Ante, currentKey, current));

                //replace current
                yield return new(ChangeReason.Remove, currentKey, new(Group.Current, currentKey, current));
                yield return new(ChangeReason.Add, valueKey, new(Group.Current, valueKey, value));


                foreach (var remain in post)
                {
                    var key = _keyFunc(remain);
                    yield return new(ChangeReason.Remove, _keyFunc(remain), new(Group.Post, key, remain));
                }


            }
        }


        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public Change<GroupedItem<T, Group, TKey>, TKey>[] AddToPosterior(T value)
        {
            post.Push(current);
            var collect = Collect(value, current).ToArray();
            ante.Clear();
            current = value;
   

            return collect;
        }

        private IEnumerable<Change<GroupedItem<T, Group, TKey>, TKey>> Collect(T value, T current)
        {
            var currentKey = _keyFunc(current ?? throw new Exception("Current can't be null before adding to Posterior"));
            var valueKey = _keyFunc(value);

            //replace current
            yield return new(ChangeReason.Remove, currentKey, new(Group.Current, currentKey, current));
            // move current to complete
            yield return new(ChangeReason.Add, currentKey, new(Group.Post, currentKey,  current));
            //}

            yield return new(ChangeReason.Add, valueKey, new(Group.Current, valueKey, value));

            foreach (var complete in ante)
            {
                var key = _keyFunc(complete);
                yield return new(ChangeReason.Remove, key, new(Group.Ante, key,  complete));
            }
        }

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public Change<GroupedItem<T, Group, TKey>, TKey>[] SelectByKey(TKey key)
        {
            var collect = Collect(key).ToArray();

            return collect;


            IEnumerable<Change<GroupedItem<T, Group, TKey>, TKey>> Collect(TKey value)
            {
                if (currentCollection.SingleOrDefault(a => _keyFunc(a).Equals(value)) is T)
                {
                    yield break;
                }
                else if (Ante.SingleOrDefault(a => _keyFunc(a).Equals(value)) is T ante)
                {
                    var currentKey = _keyFunc(Current ?? throw new Exception("Current can't be null"));
                    yield return new(ChangeReason.Add, currentKey, new(Group.Post, currentKey, Current));

                    //replace current
                    yield return new(ChangeReason.Remove, currentKey, new(Group.Current, currentKey, Current));

                    if (Ante.FirstOrDefault() is T anteFirst)
                    {
                        var anteFirstKey = _keyFunc(anteFirst);
                        yield return new(ChangeReason.Add, anteFirstKey, new(Group.Current, anteFirstKey, anteFirst));
                        yield return new(ChangeReason.Remove, anteFirstKey, new(Group.Ante, anteFirstKey, anteFirst));
                    }
                }
                else if (Post.SingleOrDefault(a => _keyFunc(a).Equals(value)) is T post)
                {
                    var currentKey = _keyFunc(Current ?? throw new Exception("Current can't be null"));
                    yield return new(ChangeReason.Add, currentKey, new(Group.Ante, currentKey, Current));

                    //replace current
                    yield return new(ChangeReason.Remove, currentKey, new(Group.Current, currentKey, Current));


                    if (Post.FirstOrDefault() is T postFirst)
                    {
                        var postFirstKey = _keyFunc(postFirst);
                        yield return new(ChangeReason.Add, postFirstKey, new(Group.Current, postFirstKey,  postFirst));
                        yield return new(ChangeReason.Remove, postFirstKey, new(Group.Post, postFirstKey, postFirst));
                    }

                    //TO DO reflect in model improve logic

                }
            }
        }


        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public IEnumerable<Change<GroupedItem<T, Group, TKey>, TKey>> MoveBackward()
        {
            var value = ante.Pop();
            var valueKey = _keyFunc(value);
            var currentKey = _keyFunc(current);
            yield return new(ChangeReason.Add, _keyFunc(value), new(Group.Current, valueKey,  value));
            yield return new(ChangeReason.Remove, _keyFunc(current ?? throw new Exception("sdfsdsd ")), new(Group.Current, currentKey, current));
            yield return new(ChangeReason.Remove, _keyFunc(value), new(Group.Ante, valueKey, value));
            yield return new(ChangeReason.Add, _keyFunc(current), new(Group.Post, currentKey, current));
            post.Push(current = value);
        }

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public IEnumerable<Change<GroupedItem<T, Group, TKey>, TKey>> MoveForeward()
        {
            var value = post.Pop();
            var valueKey = _keyFunc(value);
            var currentKey = _keyFunc(current);
            yield return new(ChangeReason.Add, _keyFunc(value), new(Group.Current, valueKey, value));
            yield return new(ChangeReason.Remove, _keyFunc(current ?? throw new Exception("sdfsdsd ")), new(Group.Current, currentKey, current));
            yield return new(ChangeReason.Remove, _keyFunc(value), new(Group.Post, valueKey, value));
            yield return new(ChangeReason.Add, _keyFunc(current), new(Group.Ante, currentKey, current));
            ante.Push(current = value);
        }

        public bool CanMoveForward()
        {
            return post.Any();
        }

        public bool CanMoveBackward()
        {
            return ante.Any();
        }
    }
}
