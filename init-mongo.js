// Инициализация MongoDB при первом запуске
// Этот скрипт выполняется только один раз при создании контейнера

try {
  // Переключаемся на рабочую БД
  db = db.getSiblingDB(process.env.MONGO_INITDB_DATABASE);
  
  // Создаём маркер инициализации
  db.createCollection('_init_marker');
  print('✓ База данных ' + process.env.MONGO_INITDB_DATABASE + ' инициализирована');
  print('✓ Инициализация MongoDB завершена успешно');
  
} catch (e) {
  print('✗ Ошибка при инициализации: ' + e.message);
  throw e;
}
