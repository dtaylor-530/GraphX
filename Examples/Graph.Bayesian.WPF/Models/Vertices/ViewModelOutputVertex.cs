using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using DynamicData;
using Graph.Bayesian.WPF.Infrastructure;
using Graph.Bayesian.WPF.Models.Vertices;
using Graph.Bayesian.WPF.ViewModel;
using ReactiveUI;

namespace Graph.Bayesian.WPF.Models
{
    public class ViewModelOutputVertex : Vertex
    {
        private readonly ReadOnlyObservableCollection<ViewModelResponse> collection;

        public ViewModelOutputVertex()
        {
            InMessages
                .OfType<ViewModelResponseMessage>()
                .Subscribe(a =>
                {
                    var (@from, to, dateTime, value) = a;
                    LastResponse = a.Response;
                    LastResponseChange = DateTime.Now;
                    OnPropertyChanged(nameof(LastResponse));
                    OnPropertyChanged(nameof(LastResponseChange));
                });

            InMessages
                .OfType<ViewModelResponseMessage>()
                .Select(a => a.Response)
                .ToObservableChangeSet()
                .Bind(out collection)
                .Subscribe();
        }

        public ViewModelResponse LastResponse { get; private set; }

        public ReadOnlyObservableCollection<ViewModelResponse> Responses => collection;

        public DateTime LastResponseChange { get; private set; }
    }


    //public class ViewModelOutputViewModel : ReactiveSubject<IChangeSet<Product>, Unit>
    //{
    //    private readonly ReadOnlyObservableCollection<Product> selections;
    //    private Product? selection;

    //    public ViewModelOutputViewModel()
    //    {
    //        In.Bind(out selections).Subscribe();
    //    }

    //    public Product? Selection
    //    {
    //        get => selection;
    //        set
    //        {
    //            if (value == null && selection != null)
    //                return;
    //            if (value == null)
    //                value = selections.FirstOrDefault();
    //            if (value == null || value == selection)
    //                return;

    //            this.RaiseAndSetIfChanged(ref selection, value with { IsSelected = true });
    //        }
    //    }

    //    public ReadOnlyObservableCollection<Product> Selections => selections;

    //    public Guid Guid { get; }
    //}



    //public class ViewModelOutputService : Service<ListEdit, IChangeSet<Product>>, IObserver<Product>
    //{
    //    readonly Subject<Product> selections = new();

    //    public ViewModelOutputService()
    //    {
    //        SourceList<Product> sourceList = new();


    //        var dis2 = In
    //                    .MergeDifferent(selections)
    //                    .Subscribe(a =>
    //                    {
    //                        try
    //                        {
    //                            var (first, second) = a;

    //                            if (second != null && first == null)
    //                                if (sourceList.Items.SingleOrDefault(ae => ae.ProductId == second.ProductId) is not { } single)
    //                                {
    //                                    sourceList.Add(second);
    //                                }
    //                                else
    //                                {
    //                                    // set all other IsSelected values to false
    //                                    if (second.IsSelected)
    //                                        foreach (var prod in sourceList.Items)
    //                                        {
    //                                            if (prod.ProductId == second.ProductId)
    //                                                sourceList.Replace(single, second);
    //                                            else
    //                                                sourceList.Replace(prod, prod with { IsSelected = false });
    //                                        }
    //                                    else
    //                                        sourceList.Replace(single, second);
    //                                }

    //                            if (first != null && second != null && second.IsSelected)
    //                            {
    //                                var ee = first switch
    //                                {
    //                                    ListEditMove move => new Action(() =>
    //                                    {
    //                                        var index = sourceList.Items.IndexOf(second);

    //                                        if (index == -1)
    //                                            return;

    //                                        sourceList.Move(index, NewMethod(index, sourceList.Count, move));
    //                                    }),
    //                                    ListEdit edit =>
    //                                      edit.ListEditType switch
    //                                      {
    //                                          ListEditType.Subtract => new Action(() => { sourceList.Add(second); }),
    //                                          ListEditType.Add => new Action(() => { sourceList.Remove(second); }),
    //                                      }
    //                                };
    //                                ee.Invoke();
    //                            }
    //                        }
    //                        catch (Exception ex)
    //                        {

    //                        }
    //                    });


    //        sourceList.Connect()
    //            .Subscribe(a =>
    //            {
    //                Out.OnNext(a);
    //            });
    //        //return new CompositeDisposable(dis, dis2);


    //        //selections.Subscribe(a =>
    //        //{
    //        //    var ae = disposable;
    //        //}, e =>
    //        //{
    //        //});
    //    }

    //    private static int NewMethod(int index, int count, ListEditMove move)
    //    {
    //        var newIndex = index + (
    //                                 move.ListEditType switch
    //                                 {
    //                                     ListEditType.Up => -1,
    //                                     ListEditType.Down => 1,
    //                                     _ => throw new Exception("dfsd")
    //                                 });


    //        return (count + newIndex) % count;

    //    }

    //    public void OnNext(Product value)
    //    {
    //        selections.OnNext(value);
    //    }
    //}
}
