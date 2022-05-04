namespace TranslateViaWeb.Elements
{
    /// <summary>
    /// Action under the element
    /// </summary>
    public interface IElementAction
    {
        /// <summary>
        /// Action under the element 
        /// </summary>
        /// <param name="number">Number element in list</param>
        void Action(int number);

        /// <summary>
        /// Action under the element 
        /// </summary>
        void Action();

        string GetAttribute(string name);
        
        /// <summary>
        /// Send imitation of click button
        /// </summary>
        /// <param name="text">Text of button</param>
        void SendKey(string text);
    }
}