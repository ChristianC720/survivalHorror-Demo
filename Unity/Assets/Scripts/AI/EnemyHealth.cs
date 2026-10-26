using System;
using Heartbeat.Combat;
using Heartbeat.Config;
using UnityEngine;
using UnityEngine.AI;

namespace Heartbeat.AI
{
    /// <summary>
    /// Salud del enemigo medida en disparos: el perfil del tipo fija cuántos aguanta
    /// (Alto 4, Rastrero 2) y cada impacto del revólver resta un disparo.
    /// Al morir se apaga la IA (que además libera el cupo de la sala), el agente y la
    /// colisión; la cápsula queda en el suelo a la espera del reciclaje.
    /// Todas las cifras vienen de <see cref="GameConfig"/>.
    /// </summary>
    [RequireComponent(typeof(EnemyAI))]
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("Referencias")]
        [SerializeField] private GameConfig config;
        [SerializeField] private EnemyAI ai;

        [Header("Depuración")]
        [Tooltip("Registra impactos y muerte en consola.")]
        [SerializeField] private bool logEvents = true;

        private int maxHealth;
        private int health;
        private bool dead;
        private bool hasBeenRecycled;

        /// <summary>Vida máxima en unidades de daño (disparos que aguanta × daño del revólver).</summary>
        public int MaxHealth => maxHealth;

        /// <summary>Vida actual en unidades de daño.</summary>
        public int Health => health;

        /// <summary>Vida restante 0-1.</summary>
        public float Health01 => maxHealth > 0 ? (float)health / maxHealth : 0f;

        /// <summary>Disparos que le quedan por encima.</summary>
        public int ShotsLeft
        {
            get
            {
                int perShot = config != null ? Mathf.Max(1, config.RevolverDamage) : 1;
                return Mathf.CeilToInt((float)health / perShot);
            }
        }

        public bool IsDead => dead;

        /// <summary>Ya volvió al nivel una vez: su siguiente cuerpo no se recicla.</summary>
        public bool HasBeenRecycled => hasBeenRecycled;

        /// <summary>El enemigo ha muerto.</summary>
        public event Action<EnemyHealth> Died;

        private void Awake()
        {
            if (ai == null) ai = GetComponent<EnemyAI>();

            if (config == null)
            {
                Debug.LogError("[IA] EnemyHealth sin GameConfig.", this);
                enabled = false;
                return;
            }

            // El tipo es un campo serializado: se puede leer sin esperar al Start de EnemyAI.
            EnemyProfile profile = config.GetEnemyProfile(ai != null ? ai.Type : EnemyType.Alto);
            int perShot = Mathf.Max(1, config.RevolverDamage);

            maxHealth = Mathf.Max(1, profile.shotsToKill) * perShot;
            health = maxHealth;
        }

        /// <summary>Impacto de bala: resta daño y mata al llegar a cero.</summary>
        public void ApplyDamage(int amount, Vector3 point)
        {
            if (dead || amount <= 0) return;

            health = Mathf.Max(0, health - amount);

            if (logEvents)
            {
                string type = ai != null ? EnemyTypeUtil.DisplayName(ai.Type) : "Enemigo";
                Debug.Log($"[IA] {name} ({type}): impacto (-{amount}); {ShotsLeft} disparos restantes.");
            }

            if (health <= 0) Die();
        }

        private void Die()
        {
            if (dead) return;

            dead = true;
            health = 0;

            if (logEvents)
            {
                string type = ai != null ? EnemyTypeUtil.DisplayName(ai.Type) : "Enemigo";
                Debug.Log($"[IA] {name} ({type}): muere.");
            }

            Died?.Invoke(this);

            // Apagar la IA dispara su OnDisable, que lo saca del registro y del cupo de la sala.
            if (ai != null) ai.enabled = false;

            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.enabled = false;

            Collider body = GetComponent<Collider>();
            if (body != null) body.enabled = false;

            // El cuerpo queda en el suelo: avisará y se reciclará, salvo que ya fuera un reciclado.
            EnemyCorpse.Create(this, ai, config, !hasBeenRecycled);
        }

        /// <summary>
        /// Devuelve la vida a un enemigo reciclado: aguanta los disparos indicados y vuelve
        /// a ser dañable. El marcado de reciclado impide que su cadáver se recicle otra vez.
        /// </summary>
        public void ResetForRecycle(int shots)
        {
            if (config == null) return;

            hasBeenRecycled = true;
            maxHealth = Mathf.Max(1, shots) * Mathf.Max(1, config.RevolverDamage);
            health = maxHealth;
            dead = false;

            Collider body = GetComponent<Collider>();
            if (body != null) body.enabled = true;

            if (logEvents)
            {
                Debug.Log($"[IA] {name}: reciclado con {maxHealth} de vida ({Mathf.CeilToInt((float)health / Mathf.Max(1, config.RevolverDamage))} disparos).");
            }
        }
    }
}
