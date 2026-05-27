# Discourse Webhook Integration - Troubleshooting

## Setup Checklist

### 1. Verify Table Creation

**Check if the table exists:**
Navigate to: `https://localhost:[PORT]/api/discourse/debug/table`

Expected response if working:
```json
{
  "tableName": "DiscourseReactions",
  "exists": true,
  "message": "Table exists"
}
```

### 2. If Table Doesn't Exist

**Option A: Restart the Application**
- Stop the application completely
- Clear the bin/obj folders if needed
- Rebuild and restart
- Check the application logs for migration messages

**Option B: Manual Database Check**
Run this SQL query on your Umbraco database:
```sql
SELECT name FROM sqlite_master WHERE type='table' AND name='DiscourseReactions';
```

**Option C: Reset Migration State**
If the migration ran but failed, delete the migration key and restart:
```sql
DELETE FROM umbracoKeyValue WHERE [key] LIKE '%DiscourseReaction%';
```

### 3. Check Application Logs

Look for these log messages on startup:
- ✅ `DiscourseReactionStoreComponent.InitializeAsync called`
- ✅ `Creating migration plan for DiscourseReactions table`
- ✅ `Executing DiscourseReaction migration plan`
- ✅ `Running DiscourseReaction migration`
- ✅ `Creating table DiscourseReactions`
- ✅ `Table DiscourseReactions created successfully`

### 4. Test the Webhook Endpoint

**Health check:**
```
GET https://localhost:[PORT]/api/discourse/health
```

**Test with sample data:**
```bash
curl -X POST https://localhost:[PORT]/api/discourse/webhook \
  -H "Content-Type: application/json" \
  -d @C:\Users\owain\Downloads\forum.json
```

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/discourse/webhook` | POST | Receives Discourse webhooks |
| `/api/discourse/recent?count=10` | GET | Get recent reactions |
| `/api/discourse/count` | GET | Get total reaction count |
| `/api/discourse/health` | GET | Health check |
| `/api/discourse/debug/table` | GET | Check if table exists |

## Common Issues

### "SQLite Error 1: 'no such table: DiscourseReactions'"

**Cause:** The migration didn't run or failed silently.

**Solutions:**
1. Check RuntimeState.Level in logs - should be "Run" not "Boot"
2. Verify ComponentComposer is being discovered
3. Check for migration exceptions in logs
4. Manually reset migration state (see Option C above)

### Migration Runs But Table Not Created

**Check:**
1. Database file permissions
2. SQLite version compatibility
3. Entity attributes (`[ExplicitColumns]`, `[Column]`, etc.)

### Webhook Returns 500 Error

**Check:**
1. Table exists (`/api/discourse/debug/table`)
2. Payload structure matches `DiscourseWebhookPayload` model
3. Application logs for detailed error

## File Structure

```
H5YR.Core/
├── Composers/
│   ├── DiscourseComposer.cs              (DI registration)
│   └── DiscourseReactionStoreComposer.cs (Migration runner)
├── Controllers/API/
│   └── DiscourseWebhookController.cs     (Webhook endpoint)
├── Data/
│   ├── Constants/
│   │   └── DiscourseReactionSchemaConstants.cs
│   ├── Entities/
│   │   └── DiscourseReaction.cs          (NPoco entity)
│   ├── Interfaces/
│   │   └── IDiscourseReactionStore.cs
│   ├── Migrations/
│   │   └── DiscourseReactionCreateTableMigration.cs
│   └── Stores/
│       └── DiscourseReactionStore.cs     (Repository)
├── Models/
│   └── Discourse/
│       └── DiscourseWebhookPayload.cs    (Webhook models)
└── Services/
    ├── IDiscourseService.cs
    └── DiscourseService.cs               (Business logic)
```

## Next Steps

Once the table is created:
1. Configure Discourse webhook URL: `https://yourdomain.com/api/discourse/webhook`
2. Test with the sample JSON file
3. Verify reactions appear in `/api/discourse/recent`
4. Integrate with the unified feed (optional)
