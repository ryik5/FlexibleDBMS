using System.Collections.Generic;

namespace FlexibleDBMS
{
    /// <summary>
    /// Originator. Создает объект хранителя своего состояния
    /// </summary>
    public class ControlState //Originator
    {
        public bool[] ControlEnabled { get; private set; }

        public void SetState(bool[] controlEnabled)
        {
            ControlEnabled = controlEnabled;
        }

        public ControlStateStore SaveState()
        {
            return new ControlStateStore(ControlEnabled);
        }

        public void RestoreStateq(ControlStateStore store)
        {
            ControlEnabled = store?.ControlEnabled;
        }
    }


    /// <summary>
    /// Memento. Хранитель объекта ControlState
    /// </summary>
    public class ControlStateStore //Memento
    {
        public bool[] ControlEnabled { get; private set; }

        public ControlStateStore(bool[] controlEnabled)
        {
            ControlEnabled = controlEnabled;
        }
    }


    /// <summary>
    /// Caretaker. Выполняет ф-цию хранения объект ControlStateStore
    /// </summary>
    public class ControlStateCaretaker //Caretaker
    {
        public Stack<ControlStateStore> History { get; private set; }
        public ControlStateCaretaker()
        {
            History = new Stack<ControlStateStore>();
        }
    }
}