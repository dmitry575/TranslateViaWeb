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
        void Action(int number = 0);

        string GetAttribute(string name);
    }
}