namespace ScrapingChallenge.Domain.ApiModels
{
    /// <summary>
    /// Menu Item model
    /// </summary>
    public class MenuItemModel
    {
        /// <summary>
        /// Gets or sets the <see cref="MenuTitle"/>
        /// </summary>
        public string MenuTitle { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MenuDescription"/>
        /// </summary>
        public string MenuDescription { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MenuSectionTitle"/>
        /// </summary>
        public string MenuSectionTitle { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DishName"/>
        /// </summary>
        public string DishName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DishDescription"/>
        /// </summary>
        public string DishDescription { get; set; }
    }
}
