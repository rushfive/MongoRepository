namespace R5.MongoRepository.IdentityMap
{
	public enum EntryState
	{
		// Exists in the session, hydrated existing aggregate from db
		Loaded,

		// Exists in the session through an add (exists only in-memory until committed)
		Added,

		// Marked as deleted in the session but still exists in db
		Deleted
	}
}
