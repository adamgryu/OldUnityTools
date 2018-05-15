using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// For use with SortedList.
/// Sorts the items by priority, with using time as a tiebreaker.
/// </summary>
public class PrioritySortingKey : IComparable {
    private static int uniqueConsecutiveTimeOrderIds = int.MinValue;

    private int priority;
    private int timestamp;

    public PrioritySortingKey(int priority) : this(priority, GetUniqueTime()) { }

    public PrioritySortingKey(int priority, int timestamp) {
        this.priority = priority;
        this.timestamp = timestamp;
    }

    public int CompareTo(object obj) {
        int compare = -1 * this.priority.CompareTo(((PrioritySortingKey)obj).priority);
        if (compare == 0) {
            return -1 * this.timestamp.CompareTo(((PrioritySortingKey)obj).timestamp);
        }
        return compare;
    }

    public override string ToString() {
        return "(Priority: " + priority + ", Time: " + timestamp + ")";
    }

    public static int GetUniqueTime() {
        uniqueConsecutiveTimeOrderIds++;
        return uniqueConsecutiveTimeOrderIds;
    }
}

/// <summary>
/// For use with SortedList where the list represents an ordered stack that manages some global state
/// that users have to register themselves with to modify.
/// </summary>
public class StackResourceSortingKey : PrioritySortingKey {

    private Action<StackResourceSortingKey> releaseResource;

    public StackResourceSortingKey(int priority, Action<StackResourceSortingKey> releaseResource) : base(priority) {
        Asserts.NotNull(releaseResource);
        this.releaseResource = releaseResource;
    }

    public void ReleaseResource() {
        releaseResource(this);
    }

    public static void Release(StackResourceSortingKey key) {
        if (key != null) {
            key.ReleaseResource();
        }
    }
}