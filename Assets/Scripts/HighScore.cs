using System.Collections;
using UnityEngine;
using System.Linq;
using System.Text;
using System;

class HighScore : IComparable<HighScore>
{
    public int Score { get; set; }

    public string Name { get; set; }

    public DateTime Date { get; set; }

    public int ID { get; set; }

    public HighScore(int id, int score, string name, DateTime date)
    {
        this.Score = score;
        this.ID = id;
        this.Name = name;
        this.Date = date;
    }

    public int CompareTo(HighScore other)
    {
        if (other.Score < this.Score)
        {
            return -1;
        }
        else if (other.Score > this.Score)
        {
            return 1;
        }
        else if (other.Date < this.Date)
        {
            return -1;
        }
        else if (other.Date > this.Date)
        {
            return 1;
        }

        return 0;
    }
}