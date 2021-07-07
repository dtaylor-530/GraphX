using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;

namespace Graph.Bayesian.WPF.Models.Vertices.History
{
    public class HistoryModel
    {
        public enum Group
        {
            Post, Ante, Current
        }
    }

    public class HistoryModel<T, TKey> : HistoryModel
       where TKey : notnull
       where T : IEquatable<T>, IKey<TKey>
    {
        IReadOnlyCollection<T> currentCollection => current == null ? Array.Empty<T>() : new[] { current };

        private readonly Stack<T> post = new();
        private readonly Stack<T> ante = new();
        private T? current;

        public HistoryModel()
        {
        }

        public IReadOnlyCollection<T> Post => post;

        public IReadOnlyCollection<T> Ante => ante;


        public T? Current => current;

        public IReadOnlyCollection<T> Collection => Ante.Concat(new[] { current }).Concat(Post).ToArray();

        public IReadOnlyCollection<TKey> Keys => Ante.Concat(currentCollection).Concat(Post).Select(a => (a.Key)).ToArray();


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
            var key = (current = value).Key;
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
                var currentKey = (current ?? throw new Exception("Current can't be null before adding to Anterior")).Key;
                var valueKey = (value).Key;
                // move current to complete
                yield return new(ChangeReason.Add, currentKey, new(Group.Ante, currentKey, current));

                //replace current
                yield return new(ChangeReason.Remove, currentKey, new(Group.Current, currentKey, current));
                yield return new(ChangeReason.Add, valueKey, new(Group.Current, valueKey, value));


                foreach (var remain in post)
                {
                    var key = (remain).Key;
                    yield return new(ChangeReason.Remove, (remain).Key, new(Group.Post, key, remain));
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
            var currentKey = (current ?? throw new Exception("Current can't be null before adding to Posterior")).Key;
            var valueKey = (value).Key;

            //replace current
            yield return new(ChangeReason.Remove, currentKey, new(Group.Current, currentKey, current));
            // move current to complete
            yield return new(ChangeReason.Add, currentKey, new(Group.Post, currentKey, current));
            //}

            yield return new(ChangeReason.Add, valueKey, new(Group.Current, valueKey, value));

            foreach (var complete in ante)
            {
                var key = (complete).Key;
                yield return new(ChangeReason.Remove, key, new(Group.Ante, key, complete));
            }
        }

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>


        public IEnumerable<Change<GroupedItem<T, Group, TKey>, TKey>> Insert(TKey key)
        {
            if (currentCollection.SingleOrDefault(a => (a.Key).Equals(key)) is T)
            {
                yield break;
            }
            else if (Ante.SingleOrDefault(a => (a.Key).Equals(key)) is T)
            {
                // remove from current and add to post
                yield return new(ChangeReason.Remove, (current ?? throw new Exception("Current can't be null")).Key, new(Group.Current, current.Key, current));
                post.Push(current);
                yield return new(ChangeReason.Add, current.Key, new(Group.Post, current.Key, current));
                //if (ante.Peek() is { Key: { } akey } && akey.Equals(key))
                //{
                //    yield return new(ChangeReason.Add, (current ?? throw new Exception("Current can't be null")).Key, new(Group.Current, current.Key, current));
                //    yield break;
                //}

                while (ante.Pop() is { } ant)
                {
                    yield return new(ChangeReason.Remove, ant.Key, new(Group.Ante, ant.Key, ant));

                    if (ant.Key.Equals(key))
                    {
                        yield return new(ChangeReason.Add, ant.Key, new(Group.Current, ant.Key, ant));
                        current = ant;
                        yield break;
                    }
                    else
                    {
                        post.Push(ant);
                        yield return new(ChangeReason.Add, ant.Key, new(Group.Post, ant.Key, ant));            
                                  
                    }
                }
            }
            else if (Post.SingleOrDefault(a => (a.Key).Equals(key)) is T)
            {
                yield return new(ChangeReason.Remove, (current ?? throw new Exception("Current can't be null")).Key, new(Group.Current, current.Key, current));
                ante.Push(current);
                yield return new(ChangeReason.Add, current.Key, new(Group.Ante, current.Key, current));

                while (post.Pop() is { } pos)
                {
                    yield return new(ChangeReason.Remove, pos.Key, new(Group.Post, pos.Key, pos));

                    if (pos.Key.Equals(key))
                    {
                   
                        yield return new(ChangeReason.Add, pos.Key, new(Group.Current, pos.Key, pos));
                        current = pos;
                        yield break;
                    }
                    else
                    {
                        ante.Push(pos);
                        yield return new(ChangeReason.Add, pos.Key, new(Group.Ante, pos.Key, pos));                   
                    }
                }
            }
        }

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public IEnumerable<Change<GroupedItem<T, Group, TKey>, TKey>> MoveBackward()
        {
            if(ante.Any()==false)
            {
                yield break;
            }
            var value = ante.Pop();
            yield return new(ChangeReason.Remove, (value).Key, new(Group.Ante, (value).Key, value));
            yield return new(ChangeReason.Add, (value).Key, new(Group.Current, (value).Key, value));
            yield return new(ChangeReason.Remove, (current ?? throw new Exception("sdfsdsd ")).Key, new(Group.Current, current.Key, current));  
            yield return new(ChangeReason.Add, (current).Key, new(Group.Post, (current).Key, current));
            post.Push(current);
            current = value;
        }

        /// <summary>
        /// removes all values from remaining and appends to complete stack
        /// </summary>
        /// <param name="value"></param>
        public IEnumerable<Change<GroupedItem<T, Group, TKey>, TKey>> MoveForeward()
        {
            if (post.Any() == false)
            {
                yield break;
            }
            var value = post.Pop();
            yield return new(ChangeReason.Remove, (value).Key, new(Group.Post, (value).Key, value));
            yield return new(ChangeReason.Add, (value).Key, new(Group.Current, (value).Key, value));
            yield return new(ChangeReason.Remove, (current ?? throw new Exception("sdfsdsd ")).Key, new(Group.Current, current.Key, current));
            yield return new(ChangeReason.Add, (current).Key, new(Group.Ante, (current).Key, current));
            ante.Push(current);
            current = value;
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
