## How much stronger is grimmchild anyway?

Well this may be a silly question since this mod is designed to let you choose the strength. But, of course, like any mod, it does have
some defaults set.

These are all relative to their default values
* Maximum attack speed multiplier: 2.5
* Maximum fireball speed multiplier: 1.5
* Maximum fireball size multiplier: 2.0
* Maximum range multiplier: 2.0

And these are static
* Maximum damage: 35
* Maximum notches cost: 6


### What's this about maximum values?

The actual strength level also depends on grimmchild's level, which in turn depends on if you're using Infinite Grimm or not. And the maximum damage you have done to Infinite Grimm. **If you aren't using Infinite Grimm, your grimmchild will be at maximum strength all the time!**

Essentialy for calculating the strength at a given level the game uses a weighted average system based on these preset values.

The numbers below represent the distance from the starting value, where 1 is using the maximum value fully and 0 is using the starting value and anything in between is in between this linearly. With the exception of notches cost which will be the costs at every level (except maximum) unless you set a lower value. And Ghost ball which will be enabled after a certain level unless disabled in config.

| GC Level Chart          | Attack Speed | Fireball speed | Range | Fireball Size | Damage | Ghost Ball | Notches Used |
|-------------------------|--------------|----------------|-------|---------------|--------|------------|--------------|
| Level 1 (0-1999 dmg)    | 0.5          | 0              | 0     | 0.1           | 0      | false      | 3            |
| Level 2 (2000-3999 dmg) | 0.6          | 0.2            | 0.2   | 0.3           | 0.2    | false      | 4            |
| Level 3 (4000-5999 dmg) | 0.7          | 0.4            | 0.4   | 0.5           | 0.4    | false      | 5            |
| Level 4 (6000-9999 dmg) | 0.8          | 0.6            | 0.6   | 0.7           | 0.6    | true       | 6            |
| Level 5 (10k-14999 dmg) | 0.9          | 0.8            | 0.8   | 0.85          | 0.8    | true       | 6            |
| Level 6 / no IG         | 1.0          | 1.0            | 1.0   | 1.0           | 1.0    | true       | 6            |

## 6 notches?!?!!!

Although this may sound like a lot, and it is, you will find maximum level grimmchild's help more than worth it. For six notches you get a creature that can dish out almost 7 times the DPS of normal grimmchild, with much better accuracy, and who can shoot through walls.
