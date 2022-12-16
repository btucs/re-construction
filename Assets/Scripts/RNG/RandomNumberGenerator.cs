/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * The Initial Developer of the Original Code is Rune Skovbo Johansen.
 * Portions created by the Initial Developer are Copyright (C) 2015
 * the Initial Developer. All Rights Reserved.
 */

using System;

public abstract class RandomNumberGenerator {

  public abstract uint Next();

  public float Value() {
    return Next() / (float)uint.MaxValue;
  }

  public int Range(int min, int max) {
    return min + (int)(Next() % (max - min));
  }

  public float Range(float min, float max) {
    return min + (Next() * (float)(max - min)) / uint.MaxValue;
  }
};
