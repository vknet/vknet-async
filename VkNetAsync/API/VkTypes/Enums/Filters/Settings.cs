namespace VkNetAsync.API.VkTypes.Enums.Filters
{
	/// <summary>
	/// Права доступа приложений.
	/// См. описание <see href="http://vk.com/pages?oid=-1&amp;p=Права_доступа_приложений"/>.
	/// </summary>
	public sealed class Settings : FilteredEnum<Settings>
	{

		/// <summary>
		/// Пользователь разрешил отправлять ему уведомления.
		/// </summary>
		public static readonly Settings Notify = RegisterPossibleValue(1 << 0, "notify");

		/// <summary>
		/// Доступ к друзьям. 
		/// </summary>
		public static readonly Settings Friends = RegisterPossibleValue(1 << 1, "friends");

		/// <summary>
		/// Доступ к фотографиям. 
		/// </summary>
		public static readonly Settings Photos = RegisterPossibleValue(1 << 2, "photos");

		/// <summary>
		/// Доступ к аудиозаписям. 
		/// </summary>
		public static readonly Settings Audio = RegisterPossibleValue(1 << 3, "audio");

		/// <summary>
		/// Доступ к видеозаписям. 
		/// </summary>
		public static readonly Settings Video = RegisterPossibleValue(1 << 4, "video");

		/// <summary>
		/// Доступ к wiki-страницам. 
		/// </summary>
		public static readonly Settings Pages = RegisterPossibleValue(1 << 7, "pages");

		/// <summary>
		/// Добавление ссылки на приложение в меню слева.
		/// </summary>
		public static readonly Settings AddLinkToLeftMenu = RegisterPossibleValue(1 << 8, "addLinkToLeftMenu");

		/// <summary>
		/// Доступ к статусу пользователя. 
		/// </summary>
		public static readonly Settings Status = RegisterPossibleValue(1 << 10, "status");

		/// <summary>
		/// Доступ заметкам пользователя. 
		/// </summary>
		public static readonly Settings Notes = RegisterPossibleValue(1 << 11, "notes");

		/// <summary>
		/// Доступ к расширенным методам работы с сообщениями. 
		/// </summary>
		public static readonly Settings Messages = RegisterPossibleValue(1 << 12, "messages");

		/// <summary>
		/// Доступ к обычным и расширенным методам работы со стеной.
		/// </summary>
		public static readonly Settings Wall = RegisterPossibleValue(1 << 13, "wall");

		/// <summary>
		/// Доступ к расширенным методам работы с рекламным API. 
		/// </summary>
		public static readonly Settings Ads = RegisterPossibleValue(1 << 15, "ads");

		/// <summary>
		/// Доступ к документам.
		/// </summary>
		public static readonly Settings Documents = RegisterPossibleValue(1 << 17, "docs");

		/// <summary>
		/// Доступ к группам пользователя. 
		/// </summary>
		public static readonly Settings Groups = RegisterPossibleValue(1 << 18, "groups");

		/// <summary>
		/// Доступ к оповещениям об ответах пользователю. 
		/// </summary>
		public static readonly Settings Notifications = RegisterPossibleValue(1 << 19, "notifications");

		/// <summary>
		/// Доступ к статистике групп и приложений пользователя, администратором которых он является. 
		/// </summary>
		public static readonly Settings Statistic = RegisterPossibleValue(1 << 20, "stats");

		/// <summary>
		/// Доступ к API в любое время со стороннего сервера. 
		/// </summary>
		public static readonly Settings Offline = RegisterPossibleValue(1 << 16, "offline");

		/// <summary>
		/// Доступ ко всем возможным операциям с ограничением по времени.
		/// </summary>
		public static readonly Settings All =
			Notify | Friends | Photos | Audio | Video | Documents | Notes | Pages | Status
			| AddLinkToLeftMenu | Wall | Groups | Messages | Notifications | Statistic | Ads;

		/// <summary>
		/// Доступ ко всем возможным операциям без ограничений по времени (т.е. с флагом <see cref="Offline"/>).
		/// </summary>
		public static readonly Settings AllOffline = All | Offline;

	}
}