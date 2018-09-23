using System.Collections.Generic;

public class ExploreCatYComparer : IComparer<ExploreCat> {
    public int Compare(ExploreCat cat1, ExploreCat cat2) {
        return (int)((cat2.gameObject.transform.position.y - cat1.gameObject.transform.position.y) * 100000);
    }
}