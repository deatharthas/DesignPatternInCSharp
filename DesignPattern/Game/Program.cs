using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            GameRunner runner = new GameRunner(game);
            runner.Run();
        }
    }

    class GameSaveStore
    {
        public IGameSave GameSave { get; set; }
    }

    class GameRunner
    {
        private Game m_game;
        private GameSaveStore m_gameSaveStore = new GameSaveStore();
        public GameRunner(Game game)
        {
            m_game = game;
        }

        public void Run()
        {
            while (!m_game.IsGameOver)
            {
                m_game.BeginNewRound();
                bool validSelection = false;
                while (!validSelection)
                {
                    m_game.ShowGameState();
                    Console.WriteLine("Make your choice: 1. attack 2. Cure 3. Save 4. Load");
                    var str = Console.ReadLine();
                    if (str.Length != 1)
                    {
                        continue;
                    }
                    switch (str[0])
                    {
                        case '1':
                            {
                                validSelection = true;
                                m_game.AttackMonster();
                                break;
                            }
                        case '2':
                            {
                                validSelection = true;
                                m_game.CurePlayer();
                                break;
                            }
                        case '3':
                            {
                                validSelection = false;
                                m_gameSaveStore.GameSave = m_game.CreateSave();
                                break;
                            }
                        case '4':
                            {
                                validSelection = false;
                                if(m_gameSaveStore.GameSave == null)
                                {
                                    Console.WriteLine("no save to load");
                                }
                                else
                                {
                                    m_game.RestoreFromGameSave(m_gameSaveStore.GameSave);
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
                if(!m_game.IsGameOver)
                {
                    m_game.AttackPlayer();
                }
            }            
        }
    }

    class Character
    {
        public int HealthPoint { get; set; }
        public int AttackPoint { get; set; }        

        public virtual void AttackChracter(Character opponent)
        {
            opponent.HealthPoint -= this.AttackPoint;
            if (opponent.HealthPoint < 0)
            {
                opponent.HealthPoint = 0;
            }
        }

        public virtual void Cure()
        {

        }
    }

    class Player : Character
    {
        private float playerCriticalPossible;
        public Player(float critical)
        {
            playerCriticalPossible = critical;
        }

        public override void AttackChracter(Character opponent)
        {
            base.AttackChracter(opponent);
            Console.WriteLine("Player Attacked Monster");

            Random r = new Random();
            bool critical = r.Next(0, 100) < playerCriticalPossible * 100;
            if (critical)
            {
                base.AttackChracter(opponent);
                Console.WriteLine("Player Attacked Monster again");
            }
        }

        public override void Cure()
        {
            Random r = new Random();
            HealthPoint += r.Next(5, 10);
            Console.WriteLine("Player cured himself");
        }
    }

    class Monster : Character
    {
        private float monsterMissingPossible;
        public Monster(float missing)
        {
            monsterMissingPossible = missing;
        }

        public override void AttackChracter(Character opponent)
        {
            Random r = new Random();
            bool missing = r.Next(0, 100) < monsterMissingPossible * 100;
            if (missing)
            {
                Console.WriteLine("Monster missed it");
            }
            else
            {
                base.AttackChracter(opponent);
                Console.WriteLine("Monster Attacked player");
            }
        }
    }

    interface IGameSave
    {

    }

    class Game
    {
        private Character m_player;
        private Character m_monster;
        private int m_round;
        private float playerCriticalPossible = 0.6f;
        private float monsterMissingPossible = 0.2f;
        
        public Game()
        {
            m_player = new Player(playerCriticalPossible)
            {
                HealthPoint = 15,
                AttackPoint = 2
            };
            m_monster = new Monster(monsterMissingPossible)
            {
                HealthPoint = 20,
                AttackPoint = 6
            };
        }

        public bool IsGameOver => m_monster.HealthPoint == 0 || m_player.HealthPoint == 0;        

        public void AttackMonster()
        {            
            m_player.AttackChracter(m_monster);
        }

        public void AttackPlayer()
        {
            m_monster.AttackChracter(m_player);
        }

        public void CurePlayer()
        {
            m_player.Cure();
        }

        public void BeginNewRound()
        {
            m_round++;            
        }

        public void ShowGameState()
        {
            Console.WriteLine("".PadLeft(20, '-'));
            Console.WriteLine("Round:{0}", m_round);
            Console.WriteLine("player health:{0}", "".PadLeft(m_player.HealthPoint, '*'));
            Console.WriteLine("monster health:{0}", "".PadLeft(m_monster.HealthPoint, '*'));
        }

        public IGameSave CreateSave()
        {
            var save = new GameSave()
            {
                PlayerHealth = m_player.HealthPoint,
                PlayerAttack = m_player.AttackPoint,
                PlayerCritialAttackPossible = playerCriticalPossible,
                MonsterAttack = m_monster.AttackPoint,
                MonsterHealth = m_monster.HealthPoint,
                MonsterMissingPossible = monsterMissingPossible,
                GameRound = m_round
            };
            Console.WriteLine("game saved");
            return save;
        }

        public void RestoreFromGameSave(IGameSave gamesave)
        {
            GameSave save = gamesave as GameSave;
            if(save != null)
            {
                m_player = new Player(save.PlayerCritialAttackPossible) { HealthPoint = save.PlayerHealth, AttackPoint = save.PlayerAttack };
                m_monster = new Player(save.MonsterMissingPossible) { HealthPoint = save.MonsterHealth, AttackPoint = save.MonsterAttack };
                m_round = save.GameRound;
            }
            Console.WriteLine("game restored");
        }

        private class GameSave : IGameSave
        {
            public int PlayerHealth { get; set; }
            public int PlayerAttack { get; set; }
            public float PlayerCritialAttackPossible { get; set; }
            public int MonsterHealth { get; set; }
            public int MonsterAttack { get; set; }
            public float MonsterMissingPossible { get; set; }
            public int GameRound { get; set; }
        }
    }
}
