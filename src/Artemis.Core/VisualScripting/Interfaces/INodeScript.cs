﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Artemis.Core
{
    public interface INodeScript : INotifyPropertyChanged, IDisposable
    {
        string Name { get; }
        string Description { get; }
        bool HasNodes { get; }

        IEnumerable<INode> Nodes { get; }

        Type ResultType { get; }

        object? Context { get; set; }
        
        event EventHandler<INode>? NodeAdded;
        event EventHandler<INode>? NodeRemoved;
        
        void Run();
        void AddNode(INode node);
        void RemoveNode(INode node);
    }

    public interface INodeScript<out T> : INodeScript
    {
        T Result { get; }
    }
}
