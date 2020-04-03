using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace AutoAnalysis
{   
    //public static class ObservableCollectionExtensions
    //{
    //    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    //    {
    //        items.ToList().ForEach(collection.Add);
    //    }
    //}


    //// http://stackoverflow.com/questions/13302933/how-to-avoid-firing-observablecollection-collectionchanged-multiple-times-when-r
    //// http://stackoverflow.com/questions/670577/observablecollection-doesnt-support-addrange-method-so-i-get-notified-for-each
    //public class ObservableCollectionFast<T> : ObservableCollection<T>
    //{
    //    public ObservableCollectionFast()            : base()
    //    {        }

    //    public ObservableCollectionFast(IEnumerable<T> collection)            : base(collection)
    //    {        }

    //    public ObservableCollectionFast(List<T> list)            : base(list)
    //    {        }

    //    public virtual void AddRange(IEnumerable<T> collection)
    //    {
    //        if (!(collection?.Count()>0))
    //            return;

    //        foreach (T item in collection)
    //        {
    //            this.Items.Add(item);
    //        }

    //        this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
    //        this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
    //        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //        // Cannot use NotifyCollectionChangedAction.Add, because Constructor supports only the 'Reset' action.
    //    }

    //    public virtual void RemoveRange(IEnumerable<T> collection)
    //    {
    //        if (!(collection?.Count() > 0))
    //            return;

    //        bool removed = false;
    //        foreach (T item in collection)
    //        {
    //            if (this.Items.Remove(item))
    //                removed = true;
    //        }

    //        if (removed)
    //        {
    //            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
    //            this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
    //            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //            // Cannot use NotifyCollectionChangedAction.Remove, because Constructor supports only the 'Reset' action.
    //        }
    //    }

    //    public virtual void Reset(T item)
    //    {
    //        this.Reset(new List<T>() { item });
    //    }

    //    public virtual void Reset(IEnumerable<T> collection)
    //    {
    //        if (!(collection?.Count() > 0) && !(this.Items?.Count() > 0))
    //            return;

    //        // Step 0: Check if collection is exactly same as this.Items
    //        if (IEnumerable<T>.Equals(collection, this.Items))
    //            return;

    //        int count = this.Count;

    //        // Step 1: Clear the old items
    //        this.Items.Clear();

    //        // Step 2: Add new items
    //        if (collection?.Count() > 0)
    //        {
    //            foreach (T item in collection)
    //            {
    //                this.Items.Add(item);
    //            }
    //        }

    //        // Step 3: Don't forget the event
    //        if (this.Count != count)
    //            this.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
    //        this.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
    //        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //    }
    //}


    ///// <summary> 
    ///// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed. 
    ///// </summary> 
    ///// <typeparam name="T"></typeparam> 
    //public class ObservableRangeCollection<T> : ObservableCollection<T>
    //{
    //    /// <summary> 
    //    /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
    //    /// </summary> 
    //    public void AddRange(IEnumerable<T> collection)
    //    {
    //        if (collection == null) throw new ArgumentNullException("collection");

    //        foreach (var i in collection) Items.Add(i);
    //        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //    }

    //    /// <summary> 
    //    /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
    //    /// </summary> 
    //    public void RemoveRange(IEnumerable<T> collection)
    //    {
    //        if (collection == null) throw new ArgumentNullException("collection");

    //        foreach (var i in collection) Items.Remove(i);
    //        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //    }

    //    /// <summary> 
    //    /// Clears the current collection and replaces it with the specified item. 
    //    /// </summary> 
    //    public void Replace(T item)
    //    {
    //        ReplaceRange(new T[] { item });
    //    }

    //    /// <summary> 
    //    /// Clears the current collection and replaces it with the specified collection. 
    //    /// </summary> 
    //    public void ReplaceRange(IEnumerable<T> collection)
    //    {
    //        if (collection == null) throw new ArgumentNullException("collection");

    //        Items.Clear();
    //        foreach (var i in collection) Items.Add(i);
    //        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //    }

    //    /// <summary> 
    //    /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
    //    /// </summary> 
    //    public ObservableRangeCollection()
    //        : base() { }

    //    /// <summary> 
    //    /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
    //    /// </summary> 
    //    /// <param name="collection">collection: The collection from which the elements are copied.</param> 
    //    /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
    //    public ObservableRangeCollection(IEnumerable<T> collection)
    //        : base(collection) { }
    //}

}
