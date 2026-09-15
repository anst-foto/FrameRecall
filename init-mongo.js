const DB_NAME = 'frame_recall_db';

try {
    db = db.getSiblingDB(DB_NAME);
    db.createCollection('_init_marker');
    print('✓ База данных ' + DB_NAME + ' инициализирована');
    print('✓ Инициализация MongoDB завершена успешно');
} catch (e) {
    print('✗ Ошибка при инициализации: ' + e.message);
    throw e;
}