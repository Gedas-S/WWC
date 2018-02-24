using System;
using System.Reflection;

namespace WetWeaponCheck
{
    public class ModuleWWC : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Deactivation Depth"),
         UI_FloatRange(controlEnabled = true, scene = UI_Scene.All, minValue = 0f, maxValue = -100f, stepIncrement = 1f)]
        public float CutoffDepth = -5;

        private float cutOffDepth = 0.0f;
        Type moduleTurret;
        Type moduleWeapon;
        FieldInfo maxPitch;
        FieldInfo yawRange;
        FieldInfo pitchSpeedDPS;
        FieldInfo engageGround;
        FieldInfo engageAir;
        FieldInfo engageMissile;
        FieldInfo engageSLW;
        FieldInfo engageRangeMin;

        PartModule turret;
        PartModule weapon;

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                weapon = weaponCheck();
                turret = turretCheck();
                getFields();
            }
            base.OnStart(state);
        }

        public override void OnUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                cutOffDepth = CutoffDepth;

                if (vessel.altitude < cutOffDepth)
                {
                    DisableWeapon();
                }
            }
            base.OnUpdate();
        }

        private PartModule weaponCheck()
        {
            PartModule weapon = null;

            using (var m = part.FindModulesImplementing<PartModule>().GetEnumerator())
                while (m.MoveNext())
                {
                    if (m.Current.moduleName == "ModuleWeapon")
                        weapon = m.Current;
                }

            return weapon;
        }

        private void getFields()
        {
            if (turret != null)
            {
                moduleTurret = turret.GetType();
                maxPitch = moduleTurret.GetField(nameof(maxPitch));
                yawRange = moduleTurret.GetField(nameof(yawRange));
                pitchSpeedDPS = moduleTurret.GetField(nameof(pitchSpeedDPS));
            }
            if (weapon != null)
            {
                moduleWeapon = weapon.GetType();
                engageGround = moduleWeapon.GetField(nameof(engageGround));
                engageAir = moduleWeapon.GetField(nameof(engageAir));
                engageMissile = moduleWeapon.GetField(nameof(engageMissile));
                engageRangeMin = moduleWeapon.GetField(nameof(engageRangeMin));
                engageSLW = moduleWeapon.GetField(nameof(engageSLW));
            }
        }

        private PartModule turretCheck()
        {
            PartModule weapon = null;

            using (var m = part.FindModulesImplementing<PartModule>().GetEnumerator())
                while (m.MoveNext())
                {
                    if (m.Current.moduleName == "ModuleTurret")
                        weapon = m.Current;
                }

            return weapon;
        }

        public void DisableWeapon()
        {
            if (turret != null)
            {
                maxPitch.SetValue(turret, 0);
                yawRange.SetValue(turret, 0);
                pitchSpeedDPS.SetValue(turret, 0);
            }
            if (weapon != null)
            {
                engageGround.SetValue(weapon, false);
                engageAir.SetValue(weapon, false);
                engageMissile.SetValue(weapon, false);
                engageSLW.SetValue(weapon, false);
                engageRangeMin.SetValue(weapon, 0);
            }
        }
    }
}
