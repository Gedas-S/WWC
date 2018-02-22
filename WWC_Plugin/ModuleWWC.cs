using BDArmory;

namespace WetWeaponCheck
{
    public class ModuleWWC : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Deactivation Depth"),
         UI_FloatRange(controlEnabled = true, scene = UI_Scene.All, minValue = 0f, maxValue = -100f, stepIncrement = 1f)]
        public float CutoffDepth = -5;

        private float cutOffDepth = 0.0f;
        ModuleTurret turret;
        ModuleWeapon weapon;

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                weapon = weaponCheck();
                turret = turretCheck();
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

        private ModuleWeapon weaponCheck()
        {
            ModuleWeapon weapon = null;

            weapon = part.FindModuleImplementing<ModuleWeapon>();

            return weapon;
        }

        private ModuleTurret turretCheck()
        {
            ModuleTurret turret = null;

            turret = part.FindModuleImplementing<ModuleTurret>();

            return turret;
        }

        public void DisableWeapon()
        {
            turret.maxPitch = 0;
            turret.yawRange = 0;
            turret.pitchSpeedDPS = 0;
            weapon.engageGround = false;
            weapon.engageAir = false;
            weapon.engageMissile = false;
            weapon.engageSLW = false;
            weapon.engageRangeMin = 0;
        }
    }
}
