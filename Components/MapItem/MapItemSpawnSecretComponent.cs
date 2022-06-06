using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MapItemInstanceComponent))]
    public class MapItemSpawnSecretComponent : MapItemComponent
    {
        #region Members

        private static readonly Vector3 tokenDisplayHeadOffset = new Vector3(0.0f, 2.0f, 0.0f);

        #endregion Members

        #region Class Methods

        public override void InitModel(MapItemModel model)
        {
            base.InitModel(model);
            if (model is BigBoomChestModel)
            {
                BigBoomChestModel bigBoomChestModel = model as BigBoomChestModel;
                if (Random.Range(0.0f, 1.0f) <= bigBoomChestModel.TrapRate)
                    _model.DieEvent += () => SpawnBoomTrap(bigBoomChestModel.Name, bigBoomChestModel.Description, bigBoomChestModel.TrapDamage, bigBoomChestModel.TrapArea, bigBoomChestModel.TimeBurst);
                else
                    _model.DieEvent += () => SpawnBigReward(bigBoomChestModel.Token);
            }
            else Destroy(this);
        }

        private void SpawnBoomTrap(string name, string info, int trapDamage, int trapArea, int timeBurst)
        {
            BombModel bombData = new BombModel(name, info, "", "", trapDamage, trapArea, 1, timeBurst, 1, "", false);
            bombData.bombType = BombType.BombBigSecret;
            GameObject bigBoomGameObject = PoolManager.GetObject(bombData.Type);
            bigBoomGameObject.GetComponent<BombInstanceComponent>().Build(bombData, transform.position);
        }

        private void SpawnBigReward(float token)
        {
            if (_model.hitBy == HitBy.HeroBomb)
                EventManager.Invoke<float, Vector3>(GameEventType.GetToken, token, _model.Position + tokenDisplayHeadOffset);
        }
    }

    #endregion Class Methods
}