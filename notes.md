# Additional Reading

- Natural Join on multiple tables
- Look into Join Types
- Look into Join Conditions
- Look at table conditions/constraints when next making a table
- Remember to brush up on unique data types in SQL

# Database Theory

- Data Manipulation Language (DML)

## Database Systems (DBMS)

- Can contain many databases
- Contains programs that allow for access to data
- Used to manage databases

## Databases

- Contain a large collectin of data
- Contains an Administrator (DBA) with central control over the system
- Collections of multiple sets/relations/tables

## Keys

- Keys are subsets of relations
- If the keys are unique (non-repeatable) then the key is a Superkey
- Candidate Keys are minimal, meaning that if an attribute can be removed and it is still unique, it is not the minimally viable unique key
- Primary keys are the keys you use to identify domains
- Foreign keys are attributes that appear in other relations
- 

# Data Models

- Collection of concepts and tools for describing data, data relations, data semantics and data constraints

## Examples of models

- Relationa Model
- ER Model
- Object based data models
- Semi-structured data models (such as JSON)

## Relational Model

- Ted Codd
- Relations are Tables
- Attributes are the Collumns
- Domains are the types of values contained in collumns
- NULL is a memeber of all domains, i.e. all value types can be empty
- Relations are sets, they are unordered by default
- There are no duplicate values in an ordered set

### Relation Schema

- A functional way of displaying what collumns are in a table, i.e. what attributes are in a relation

### Relational ALgebra

# DML

- Divided into Pure and Commercial languages
    - SQL is Commercial
    - Relational Algebra is Pure

# ORM

- Object Relational Mapping
- For mapping between database data and objects in an application

# Database Engine

## Query Processor

- Most of the database efficiency is found in the DBMS' query processor
- Queries are taken from the application and turned into algorithms
    - The service that converts queries into algorithms is called a Query Optimizer

## Transaction Manager

- 

## Storage Manager

# Entity Relations

- Cardinality refers to the number of entities relating to each other, it is denoted by a straight line in one of three ways:
    - One to One
    - One to Many (Denoted by an Arrow at the end point)
    - Many to One (Denoted by an Arrow at the starting point)

- Specialization is like class diagrams, you define a general set of attributes for others tables to inherit
    - Inheritance can be Disjointed or Overlapping
        - Disjointed means inheritance can be either or
        - Overlapping means inheritance can be both
    - Specialization is defined by an empty/hollow arrow from the specialization to the inheritant

