using System;
using System.Reactive;
using Graph.Bayesian.WPF.Models;
using Graph.Bayesian.WPF.Models.Vertices;
using Graph.Bayesian.WPF.ViewModel;

namespace Graph.Bayesian.WPF.Infrastructure
{

    public record Message(string From, string To, DateTime Sent, object Content);

    public record ReceivedMessage(string From, string To) : Message(From, To, DateTime.Now, Unit.Default);

    public record ProceedMessage(string From, string To) : Message(From, To, DateTime.Now, Unit.Default);

    public record HistoryMessage<T>(string From, string To, DateTime Sent, /*DateTime Received,*/ T Data) : DataMessage<T>(From, To, Sent, /*DateTime Received,*/ Data);

    public record DataMessage<T>(string From, string To,/* double Rate,*/ DateTime Sent, /*DateTime Received,*/ T Data) : Message(From, To, Sent, Data) where T : notnull;

    //public record RateMessage(string From, string To,/* double Rate,*/ DateTime Sent, double Rate) : Message(From, To, Sent, Rate)

    public record MovementMessage(string From, string To,/* double Rate,*/ DateTime Sent, Movement Movement) : Message(From, To, Sent, Movement);

    public record TimerMessage(string From, string To, DateTime DateTime, double Rate) : Message(From, To, DateTime.Now, Unit.Default), IRate;

    public record ClickMessage(string From, string To) : Message(From, To, DateTime.Now, Unit.Default);

    public record IdMessage(string From, string To, Type Type) : Message(From, To, DateTime.Now, Type);

    public record IsSelectedMessage(string From, string To, bool Value) : Message(From, To, DateTime.Now, Value);

    public record VertexSelectedMessage(string From, string To, Vertex Value) : Message(From, To, DateTime.Now, Value);

    public record NavigateVertexMessage(string From, string To, Vertex Value) : Message(From, To, DateTime.Now, Value);

    public record ProductMessage(string From, string To, Product Value) : Message(From, To, DateTime.Now, Value);

    public record OrderMessage(string From, string To, DateTime Sent, Order Order) : Message(From, To, Sent, Order);

    public record SelectionRequestMessage(string From, string To, DateTime Sent, SelectionRequest Request) : Message(From, To, Sent, Request);

    public record ViewModelResponseMessage(string From, string To, DateTime Sent, ViewModelResponse Response) : Message(From, To, Sent, Response);


}
