-- insert default data

INSERT INTO "EmailState" ("Id", "Name", "CanSend") VALUES (1, 'Draft', true);
INSERT INTO "EmailState" ("Id", "Name", "CanSend") VALUES (2, 'ToBeSent', false);
INSERT INTO "EmailState" ("Id", "Name", "CanSend") VALUES (3, 'Sent', false);

INSERT INTO "VariableType" ("Id", "Name") VALUES (1, 'InputText');
INSERT INTO "VariableType" ("Id", "Name") VALUES (2, 'AutoText');
INSERT INTO "VariableType" ("Id", "Name") VALUES (3, 'List');
INSERT INTO "VariableType" ("Id", "Name") VALUES (4, 'Translation');