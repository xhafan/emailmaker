-- insert default data

INSERT INTO EmailState (Id, Name, CanSend) VALUES (1, 'Draft', 1);
INSERT INTO EmailState (Id, Name, CanSend) VALUES (2, 'ToBeSent', 0);
INSERT INTO EmailState (Id, Name, CanSend) VALUES (3, 'Sent', 0);

INSERT INTO VariableType (id, name) VALUES (1, 'InputText');
INSERT INTO VariableType (id, name) VALUES (2, 'AutoText');
INSERT INTO VariableType (id, name) VALUES (3, 'List');
INSERT INTO VariableType (id, name) VALUES (4, 'Translation');