using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// For use with SortedList.
/// Sorts the items by priority, with using time as a tiebreaker.
/// </summary>
public struct PrioritySortingKey : IComparable {
    private int priority;
    private int time;

    public PrioritySortingKey(int priority, int time) {
        this.priority = priority;
        this.time = time;
    }

    public int CompareTo(object obj) {
        int compare = -1 * this.priority.CompareTo(((PrioritySortingKey)obj).priority);
        if (compare == 0) {
            return -1 * this.time.CompareTo(((PrioritySortingKey)obj).time);
        }
        return compare;
    }

    public override string ToString() {
        return "(Priority: " + priority + ", Time: " + time + ")";
    }
}
