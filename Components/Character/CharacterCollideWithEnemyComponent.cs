using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZB.Gameplay
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterInstanceComponent))]
    public class CharacterCollideWithEnemyComponent : CharacterComponent
    {
        #region Members

        protected static readonly float checkCollideDelay = 0.1f;
        protected List<EnemyCollideData> _enemiesCollideData;

        #endregion Members

        #region API Methods

        protected virtual void Start()
        {
            StartCoroutine(CheckCollide());
        }

        #endregion API Methods

        #region Class Methods

        public override void InitModel(CharacterModel model)
        {
            base.InitModel(model);
            _enemiesCollideData = new List<EnemyCollideData>();
            _model.WinEvent += OnReset;
            _model.LoseEvent += OnReset;
            _model.DieEvent += OnReset;
        }

        protected virtual IEnumerator CheckCollide()
        {
            while (true)
            {
                yield return new WaitForSeconds(checkCollideDelay);

                if (_model.isActivated && !_model.IsDead)
                    CheckCollideWithEnemies();
            }
        }

        protected virtual void CheckCollideWithEnemies()
        {
            foreach (var enemy in EntitiesManager.Instance.Enemies)
            {
                if (Vector3.SqrMagnitude(enemy.Position - transform.position) <= (enemy.BoundRadius + _model.BoundRadius) * (enemy.BoundRadius + _model.BoundRadius))
                {
                    if (!_enemiesCollideData.Any(x => x.enemy == enemy))
                    {
                        if (enemy.CanAttack && MapManager.IsEmptyPosition(enemy.Position) && Vector3.Dot(enemy.Direction, _model.Direction) < EnemyModel.FaceToFaceDotValue)
                            enemy.AttackEvent.Invoke();

                        var enemyCollideData = new EnemyCollideData(enemy as EnemyModel, gameObject);
                        _enemiesCollideData.Add(enemyCollideData);
                        enemyCollideData.StartCollide();
                    }
                }
                else
                {
                    if (_enemiesCollideData.Any(x => x.enemy == enemy))
                    {
                        var enemyCollideData = _enemiesCollideData.First(x => x.enemy == enemy);
                        _enemiesCollideData.Remove(enemyCollideData);
                        enemyCollideData.StopCollide();
                    }
                }
            }
        }

        protected virtual void OnReset()
        {
            _enemiesCollideData.ForEach(x => x.Reset());
            StopAllCoroutines();
        }

        #endregion Class Methods
    }

    public class EnemyCollideData
    {
        #region Members

        private static readonly float recollideDelay = 1.0f;

        public EnemyModel enemy;
        public GameObject collidedTarget;
        private int recollideLTUniqueID;

        #endregion Members

        #region Class Methods

        public EnemyCollideData(EnemyModel enemy, GameObject collidedTarget)
        {
            this.enemy = enemy;
            this.collidedTarget = collidedTarget;
            this.enemy.DieEvent += OnEnemyCollideDie;
        }

        public void StartCollide()
        {
            recollideLTUniqueID = LeanTween.delayedCall(recollideDelay, Recollide).uniqueId;
            IEntityGetHit hitEntity = collidedTarget.GetComponent<IEntityGetHit>();
            hitEntity.GetHit(enemy.CollideDamage, HitBy.Enemy);
        }

        public void Recollide()
        {
            recollideLTUniqueID = LeanTween.delayedCall(recollideDelay, Recollide).uniqueId;
            IEntityGetHit hitEntity = collidedTarget.GetComponent<IEntityGetHit>();
            hitEntity.GetHit(enemy.CollideDamage, HitBy.Enemy);
        }

        public void StopCollide()
        {
            enemy.DieEvent -= OnEnemyCollideDie;
            enemy = null;
            collidedTarget = null;
            LeanTween.cancel(recollideLTUniqueID);
        }

        public void Reset()
        {
            if (enemy != null)
            {
                enemy.DieEvent -= OnEnemyCollideDie;
                enemy = null;
                collidedTarget = null;
                LeanTween.cancel(recollideLTUniqueID);
            }
        }

        private void OnEnemyCollideDie()
        {
            enemy = null;
            collidedTarget = null;
            LeanTween.cancel(recollideLTUniqueID);
        }

        #endregion Class Methods
    }
}